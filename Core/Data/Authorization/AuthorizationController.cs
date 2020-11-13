using Core.Controller;
using Core.Data.Session;
using MongoDB.Driver;

namespace Core.Data.Authorization
{
    public class AuthorizationController : DatabaseController<AuthorizationController, AuthorizationData>
    {

        public bool TryGet(SessionData sessionData, out AuthorizationData authorizationData)
        {
            authorizationData = Collection.FindSync(x =>
                    x.SessionToken.Equals(sessionData.SessionToken) && x.UserAgent.Equals(sessionData.UserAgent))
                .FirstOrDefault();

            return authorizationData != null;
        }
        
    }
}