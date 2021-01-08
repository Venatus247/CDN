using System;
using Commons;
using Core.Controller;
using Core.Data.Account.FileStorage;
using Core.Data.Session;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Core.Data.Account
{
    public class AccountController : DatabaseController<AccountController, Account>
    {
        protected AccountController() {}

        public bool TryGetById(long id, out Account account)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool TryGetByAuthData(SessionData sessionData, out Account account)
        {
            account = Collection.FindSync(x => x.AuthorizedClients.Contains(sessionData)).FirstOrDefault();
            return account != null;
        }

        private bool AccountMatchesAuthData(Account account, SessionData sessionData)
        {
            return account.IsAuthorized(sessionData);
        }

        public async void DebugAddFileToCloud(Account account)
        {
            account.CloudFolder.Content.Add(new StorageFile("c1d562f3-7502-4c7c-847b-7c04dc413eb9"));
            await Collection.FindOneAndReplaceAsync(x => x.Id.Equals(account.Id), account);
        }
        
    }
}