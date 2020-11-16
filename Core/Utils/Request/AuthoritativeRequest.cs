using System.Text.Json.Serialization;
using Core.Data.Account;
using Core.Data.Session;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Utils.Request
{
    public abstract class AuthoritativeRequest : BasicRequest
    {
        [JsonIgnore]
        public abstract AccessLevel RequiredAccessLevel { get; protected set; }

        public SessionData SessionData { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        private Account _account { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public Account Account
        {
            get
            {
                if (_account == null && AccountController.Instance.TryGetByAuthData(SessionData, out var account))
                    _account = account;
                
                return _account;
            }
            private set => _account = value;
        }

        public override bool IsComplete()
        {
            return SessionData != null && SessionData.IsComplete();
        }

        public bool IsAuthorized()
        {
            return IsComplete() && Account != null && RequiredAccessLevel <= Account.AccessLevel;
        }
    }
}