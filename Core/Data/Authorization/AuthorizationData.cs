using Core.Controller;

namespace Core.Data.Authorization
{
    public class AuthorizationData : IIdentified
    {
        public long Id { get; set; }
        public string SessionToken { get; set; }
        public string UserAgent { get; set; }
    }
}