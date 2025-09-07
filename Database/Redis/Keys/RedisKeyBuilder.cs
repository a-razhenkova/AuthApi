using Database.Redis;
using Infrastructure;

namespace Database
{
    public class RedisKeyBuilder
    {
        private const string KeyPrefix = "auth_api";

        private readonly RedisKey _key;
        private readonly object[]? _keyIds;

        public RedisKeyBuilder(RedisKey key, object[]? keyIds = null)
        {
            _key = key;
            _keyIds = keyIds;
        }

        public virtual string BuildKey()
        {
            string keySuffix = CreateKeySuffix(_key);
            string keyName = _key.GetDescription();
            return $"{KeyPrefix}:{keyName}:{keySuffix}";
        }

        private string CreateKeySuffix(RedisKey key)
        {
            switch (key)
            {
                case RedisKey.OneTimePassword:
                    {
                        return GetId<string>((int)TwoFactorAuthIds.ExternalUserId);
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private TId GetId<TId>(int index)
            => (TId)(_keyIds?[index] ?? throw new InvalidOperationException());
    }
}