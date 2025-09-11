using AutoMapper;
using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace Business
{
    public class ClientService : IClientProcessor
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly AuthDbContext _authDbContext;
        private readonly IMapper _mapper;
        private readonly IReportProvider _reportProvider;

        public ClientService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                            AuthDbContext authDbContext,
                            IMapper mapper,
                            IReportProvider reportProvider)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _authDbContext = authDbContext;
            _mapper = mapper;
            _reportProvider = reportProvider;
        }

        public async Task<PaginatedReport<ClientDto>> SearchAsync(ClientSearchParams clientSearchParams, CancellationToken cancellationToken)
        {
            IQueryable<Client> searchQuery = _authDbContext.Client;

            if (!string.IsNullOrWhiteSpace(clientSearchParams.Key))
            {
                searchQuery = searchQuery.Where(c => c.Key == clientSearchParams.Key);
            }

            if (!string.IsNullOrWhiteSpace(clientSearchParams.Name))
            {
                searchQuery = searchQuery.Where(c => EF.Functions.Like(c.Name, $"%{clientSearchParams.Name}%"));
            }
            if (clientSearchParams.Status is not null)
            {
                searchQuery = searchQuery.Where(c => c.Status.Value == clientSearchParams.Status);
            }

            if (clientSearchParams.CanNotifyParty is not null)
            {
                searchQuery = searchQuery.Where(c => c.Right.CanNotifyParty == clientSearchParams.CanNotifyParty);
            }

            searchQuery = searchQuery
                .Include(c => c.Status)
                .Include(c => c.Right)
                .Include(c => c.Subscriptions
                               .Where(s => s.Subscription.ExpirationDate.Date >= DateTime.UtcNow.Date)
                               .OrderByDescending(s => s.Id))
                    .ThenInclude(cs => cs.Subscription)
                    .ThenInclude(s => s.Contract)
                .OrderByDescending(c => c.Id);

            return await _reportProvider.PreparePaginatedReport<Client, ClientDto>(searchQuery, clientSearchParams, cancellationToken);
        }

        public async Task<ClientDto> LoadAsync(string key)
        {
            Client client = await _authDbContext.Client.AsNoTracking()
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<string> RegisterAsync(ClientDto clientDto)
        {
            Client client = _mapper.Map<Client>(clientDto);
            client.Key = ClientKeyGenerator.CreateNewKey();
            client.Secret = ClientSecretGenerator.CreateNewSecret();

            await _authDbContext.AddAsync(client);
            await _authDbContext.SaveChangesAsync();

            return client.Key;
        }

        public async Task UpdateAsync(string key, ClientDto clientDto)
        {
            Client updatedClient = await _authDbContext.Client
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Right)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found");

            Client currentClient = updatedClient.DeepCopy();
            updatedClient = _mapper.Map(clientDto, updatedClient);

            if (!updatedClient.IsEqual(currentClient))
            {
                await _authDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string key)
        {
            Client client = await _authDbContext.Client
                .Where(c => c.Key == key)
                .Include(c => c.Status)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            if (client.Subscriptions.Any())
            {
                client.Status.Value = ClientStatuses.Disabled;
                client.Status.Reason = ClientStatusReasons.None;
                await _authDbContext.SaveChangesAsync();
            }
            else
            {
                _authDbContext.Remove(client);
                await _authDbContext.SaveChangesAsync();
            }
        }

        public async Task<string> LoadSecretAsync(string key)
        {
            Client client = await _authDbContext.Client.AsNoTracking()
                .Where(c => c.Key == key)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            return client.Secret;
        }

        public async Task<string> RefreshSecretAsync(string key)
        {
            Client client = await _authDbContext.Client
                .Where(c => c.Key == key)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            client.Secret = ClientSecretGenerator.CreateNewSecret();

            await _authDbContext.SaveChangesAsync();

            return client.Secret;
        }

        public async Task RenewSubscription(string clientKey, DateTime expirationDate, IFormFile file)
        {
            if (expirationDate <= DateTime.UtcNow.Date)
            {
                throw new BadRequestException("Invalid expiration date.");
            }

            string fileExtension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(fileExtension)
                || !fileExtension.Equals(FileExtensions.Pdf))
            {
                throw new BadRequestException("Invalid file type.");
            }

            Client client = await _authDbContext.Client
                .Where(c => c.Key == clientKey)
                .Include(c => c.Status)
                .Include(c => c.Subscriptions)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            DateTime signTimestamp = DateTime.UtcNow;
            string fileName = $"{client.Key}_{signTimestamp:yyyyMMddHHmmssfff}{fileExtension}";
            string contractPath = Path.Combine(_appSettingsOptions.ClientSubscriptionContractDirectory, signTimestamp.Year.ToString(), fileName);

            FileExtensions.EnsureFileDirectoryExists(contractPath);

            try
            {
                using FileStream content = File.Create(contractPath);
                await file.CopyToAsync(content);

                client.Subscriptions.Add(new ClientSubscription()
                {
                    Subscription = new Subscription()
                    {
                        CreateTimestamp = expirationDate.Date,
                        ExpirationDate = expirationDate.Date,
                        Contract = new Document()
                        {
                            SignTimestamp = signTimestamp,
                            Name = fileName,
                            Checksum = content.ComputeMd5Checksum(),
                            Type = DocumentTypes.SubscriptionContract
                        }
                    }
                });

                if (client.Status.Reason == ClientStatusReasons.ExpiredSubscription)
                {
                    client.Status.Value = ClientStatuses.Active;
                    client.Status.Reason = ClientStatusReasons.None;
                }

                await _authDbContext.SaveChangesAsync();
            }
            catch
            {
                if (File.Exists(contractPath))
                    File.Delete(contractPath);

                throw;
            }
        }

        public async Task<FileDto> DownloadContractAsync(string clientKey, int contractId)
        {
            Document contract = await _authDbContext.ClientSubscription.AsNoTracking()
                .Where(c => c.Client.Key.Equals(clientKey)
                         && c.Subscription.Contract.Id == contractId
                         && c.Subscription.Contract.Type == DocumentTypes.SubscriptionContract)
                .Include(c => c.Client)
                .Include(c => c.Subscription).ThenInclude(s => s.Contract)
                .Select(c => c.Subscription.Contract)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Contract not found.");

            string contractPath = Path.Combine(_appSettingsOptions.ClientSubscriptionContractDirectory, contract.SignTimestamp.Year.ToString(), contract.Name);

            return new FileDto()
            {
                Name = contract.Name,
                Content = await File.ReadAllBytesAsync(contractPath)
            };
        }
    }
}