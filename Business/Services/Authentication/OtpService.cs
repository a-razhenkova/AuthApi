﻿using Database;
using Database.AuthDb;
using Database.AuthDb.DefaultSchema;
using Database.Redis;
using Google.Authenticator;
using Infrastructure;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Business
{
    public class OtpService : IOtpProvider
    {
        private readonly AppSettingsOptions _appSettingsOptions;
        private readonly AuthDbContext _authDbContext;
        private readonly IRedisProvider _redis;

        public OtpService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                         AuthDbContext authDbContext,
                         IRedisProvider redis)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _authDbContext = authDbContext;
            _redis = redis;
        }

        public async Task<string> CreateAndSendOtpAsync(User user)
        {
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            string otp = twoFactorAuthenticator.GetCurrentPIN(user.OtpSecret, false);

            var otpDto = new OneTimePasswordDto()
            {
                UserExternalId = user.ExternalId,
                Otp = otp,
                WrongAuthAttemptsCounter = 0
            };

            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_appSettingsOptions.Security.MultiFactorAuth.LifetimeSeconds));
            await _redis.AddOrUpdateAsync(RedisKey.OneTimePassword, otpDto, cacheEntryOptions, keyIds: user.ExternalId);

            // TODO: send OTP to user via email

            return user.ExternalId;
        }

        public async Task<User> ValidateOtpAsync(string userExternalId, string otp)
        {
            OneTimePasswordDto otpDto = await _redis.LoadAsync<OneTimePasswordDto>(RedisKey.OneTimePassword, userExternalId)
                ?? throw new UnauthorizedException("The code has expired.");

            if (!otpDto.Otp.Equals(otp))
            {
                await ProcessLoginAttemptAsync(otpDto, isLoginSuccessful: false);
                throw new UnauthorizedException("Invalid code.");
            }

            User user = await _authDbContext.User
               .Where(u => u.ExternalId == userExternalId)
               .Include(u => u.Status)
               .Include(u => u.Login)
               .SingleOrDefaultAsync() ?? throw new UnauthorizedException("The code has expired.");

            if (user.Status.IsAuthAllowed())
                throw new ForbiddenException($"User status is {user.Status.Value}.");

            bool isPinValid = new TwoFactorAuthenticator().ValidateTwoFactorPIN(user.OtpSecret, otp, true);

            await ProcessLoginAttemptAsync(otpDto, isPinValid);

            if (!isPinValid)
                throw new UnauthorizedException("Invalid code.");

            return user;
        }

        private async Task ProcessLoginAttemptAsync(OneTimePasswordDto otpDto, bool isLoginSuccessful)
        {
            if (isLoginSuccessful)
            {
                await _redis.DeleteAsync(RedisKey.OneTimePassword, otpDto.UserExternalId);
            }
            else
            {
                otpDto.WrongAuthAttemptsCounter++;

                if (otpDto.WrongAuthAttemptsCounter >= _appSettingsOptions.Security.MultiFactorAuth.DefaultMaxWrongLoginAttemptsBeforeBlock)
                {
                    await _redis.DeleteAsync(RedisKey.OneTimePassword, otpDto.UserExternalId);
                }
                else
                {
                    await _redis.UpdateAsync(RedisKey.OneTimePassword, otpDto, otpDto.UserExternalId);
                }
            }
        }
    }
}