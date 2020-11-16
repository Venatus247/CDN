using System;
using Core.Controller;

namespace Core.Data.Account
{
    [Serializable]
    public class AccountReference : IMapped
    {
        public long AccountId { get; set; }
        
        public AccountReference(long accountId)
        {
            AccountId = accountId;
        }
    }
}