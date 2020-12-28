using Core.Controller;

namespace Core.Data.Session
{
    public class SessionData
    {
        public string SessionToken { get; set; }
        public string UserAgent { get; set; }

        public bool IsComplete()
        {
            return SessionToken != null;
        }
    }
}