using System.Collections.Generic;

namespace Core.Data.Account.FileStorage
{
    public class StorageFolder : StorageItem
    {
        public List<StorageFolder> SubDirectories { get; set; } = new List<StorageFolder>();
        public List<StorageFile> Content { get; set; } = new List<StorageFile>();
    }
}