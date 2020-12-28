using System;
using API.Controllers;

namespace Core.Utils
{
    public class UidGenerator : Singleton<UidGenerator>
    {

        protected UidGenerator()
        {
        }

        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
        
    }
}