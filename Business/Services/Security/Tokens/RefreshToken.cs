using Database.IdentityDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;

namespace Business
{
    public class RefreshToken : SecurityToken
    {
        public RefreshToken(SecurityOptions options)
            : base(options, options.RefreshToken.Key)
        {

        }

        public string Create<TCaller>(TCaller caller)
        {
            IToken token = caller switch
            {
                User user => new UserRefreshToken(user, _options.RefreshToken),
                _ => throw new NotImplementedException()
            };

            return new TokenStrategy(token, _options).Create();
        }
    }
}