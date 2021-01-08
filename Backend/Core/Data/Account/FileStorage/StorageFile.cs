using System;
using System.Text.Json.Serialization;
using Core.Data.File;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Data.Account.FileStorage
{
    public class StorageFile : StorageItem
    {
        public string FileId { get; set; }
        
        [JsonIgnore]
        [BsonIgnore]
        private SavedFile _file { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public SavedFile File
        {
            get { return _file ??= getFile(); }
        }

        protected StorageFile() {}

        public StorageFile(string fileId)
        {
            FileId = fileId;
        }
        
        private SavedFile getFile()
        {
            throw new NotImplementedException();
        }
    }
}