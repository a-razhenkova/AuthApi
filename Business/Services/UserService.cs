using AutoMapper;
using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.RegularExpressions;

namespace Business
{
    public class UserService : IUserProcessor
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly AuthDbContext _authDbContext;
        private readonly IMapper _mapper;
        private readonly IReportProvider _reportProvider;

        public UserService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                          AuthDbContext authDbContext,
                          IMapper mapper,
                          IReportProvider reportProvider)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _authDbContext = authDbContext;
            _mapper = mapper;
            _reportProvider = reportProvider;
        }

        public async Task<PaginatedReport<UserDto>> SearchAsync(UserSearchParams userSearchParams, CancellationToken cancellationToken)
        {
            IQueryable<User> searchQuery = _authDbContext.User;

            if (!string.IsNullOrWhiteSpace(userSearchParams.Username))
            {
                searchQuery = searchQuery.Where(u => EF.Functions.Like(u.Username, $"%{userSearchParams.Username}%"));
            }

            if (userSearchParams.Role is not null)
            {
                searchQuery = searchQuery.Where(u => u.Role == userSearchParams.Role);
            }

            if (userSearchParams.Status is not null)
            {
                searchQuery = searchQuery.Where(u => u.Status.Value == userSearchParams.Status);
            }

            searchQuery = searchQuery
                .Include(u => u.Status)
                .OrderByDescending(u => u.Id);

            return await _reportProvider.PreparePaginatedReport<User, UserDto>(searchQuery, userSearchParams, cancellationToken);
        }

        public async Task<UserDto> LoadAsync(string userExternalId)
        {
            User user = await _authDbContext.User.AsNoTracking()
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> RegisterAsync(UserDto userDto)
        {
            ValidateUserPassword(userDto.Password);

            User? user = await _authDbContext.User.AsNoTracking()
                .Where(u => u.Username == userDto.Username)
                .SingleOrDefaultAsync();

            if (user is not null)
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");

            user = _mapper.Map<User>(userDto);
            user.ExternalId = Guid.NewGuid().ToString();
            user.Password = UserPasswordProtector.CreateNewSecurePassword(userDto.Password);
            user.OtpSecret = UserSecretGenerator.CreateNewBase64Secret();

            await _authDbContext.AddAsync(user);
            await _authDbContext.SaveChangesAsync();

            return user.ExternalId;
        }

        public async Task UpdateAsync(string userExternalId, UserDto userDto)
        {
            User updatedUser = await _authDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!updatedUser.Username.Equals(userDto.Username)
                && await _authDbContext.User.AsNoTracking().Where(u => u.Username == userDto.Username).AnyAsync())
            {
                throw new ConflictException($"User with username '{userDto.Username}' already registered.");
            }

            User currentUser = updatedUser.DeepCopy();
            updatedUser = _mapper.Map(userDto, updatedUser);

            if (!updatedUser.IsEqual(currentUser))
            {
                await _authDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(string userExternalId)
        {
            User user = await _authDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            _authDbContext.Remove(user);
            await _authDbContext.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string userExternalId, string oldPassword, string newPassword)
        {
            ValidateUserPassword(newPassword);

            User user = await _authDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!UserPasswordProtector.IsPasswordValid(user.Password.Value, oldPassword, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Password = UserPasswordProtector.CreateNewSecurePassword(newPassword);

            await _authDbContext.SaveChangesAsync();

            // TODO: send information email async
        }

        public async Task ChangeEmailAsync(string userExternalId, string email, string password)
        {
            ValidateUserPassword(password);

            User user = await _authDbContext.User
                .Where(u => u.ExternalId == userExternalId)
                .Include(u => u.Status)
                .Include(u => u.Password)
                .SingleOrDefaultAsync() ?? throw new NotFoundException("User not found.");

            if (!UserPasswordProtector.IsPasswordValid(user.Password.Value, password, user.Password.Secret))
                throw new BadRequestException("Invalid old password.");

            user.Email = email;
            user.IsVerified = false;
            user.Status.Value = UserStatuses.Restricted;
            user.Status.Reason = UserStatusReasons.EmailChanged;

            await _authDbContext.SaveChangesAsync();
        }

        private void ValidateUserPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("Password is required.");

            if (!new Regex(_appSettingsOptions.Security.PasswordValidationRegex).Match(password).Success)
                throw new BadRequestException($"Password must match the regular expression '{_appSettingsOptions.Security.PasswordValidationRegex}'.");
        }
    }
}