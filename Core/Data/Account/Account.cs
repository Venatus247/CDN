using System.Collections.Generic;
using Core.Controller;
using Core.Data.Session;
using Core.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Data.Account
{
    public class Account : IIdentified
    {
        [BsonId(IdGenerator = typeof(BsonIncrementGenerator<Account>))]
        public long Id { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public MailAddress CurrentAddress { get; set; }
        public List<MailAddress> AddressHistory { get; private set; } = new List<MailAddress>();
        public List<SessionData> AuthorizedClients { get; private set; } = new List<SessionData>();
        public List<DetailedSessionData> DetailedSessionInfos { get; private set; } = new List<DetailedSessionData>();
        
        public void ChangeAddress(MailAddress newAddress)
        {
            //put old address to history
            AddressHistory.Add(CurrentAddress);

            //set new address to current
            CurrentAddress = newAddress;
        }

        public bool IsAuthorized(SessionData authData)
        {
            return AuthorizedClients.Exists(x => x.Equals(authData));
        }
        
    }
}