using Core.Controller;

namespace Core.Data.Account.FileStorage
{
    public abstract class StorageItem : IIdentified
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}