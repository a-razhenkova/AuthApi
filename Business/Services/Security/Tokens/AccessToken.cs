using Database.IdentityDb.DefaultSchema;
using Infrastructure.Configuration.AppSettings;

namespace Business
{
    public class AccessToken : SecurityToken
    {
        public AccessToken(SecurityOptions options)
            : base(options, options.AccessToken.Key)
        {

        }

        public string Create<TCaller>(TCaller caller)
        {
            IToken token = caller switch
            {
                User user => new UserAccessToken(user, _options.AccessToken),
                Client client => new ClientAccessToken(client, _options.AccessToken),
                _ => throw new NotImplementedException()
            };

            return new TokenStrategy(token, _options).Create();
        }
    }
}