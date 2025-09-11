using AutoMapper;
using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Business
{
    public class ClientService : IClientProcessor
    {
        private readonly AuthDbContext _authDbContext;
        private readonly IMapper _mapper;
        private readonly IReportProvider _reportProvider;

        public ClientService(AuthDbContext authDbContext,
                            IMapper mapper,
                            IReportProvider reportProvider)
        {
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
                .SingleOrDefaultAsync() ?? throw new NotFoundException("Client not found.");

            _authDbContext.Remove(client);
            await _authDbContext.SaveChangesAsync();
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
    }
}