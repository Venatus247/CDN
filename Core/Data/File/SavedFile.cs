using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Commons.Utils;
using Core.Data.Account;
using Core.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Data.File
{
    public class SavedFile : ICdnFile
    {
        [BsonId(IdGenerator = typeof(BsonIncrementGenerator<SavedFile>))]
        public long Id { get; set; }
        public string FileId { get; set; }
        public string FileName
        {
            get => _fileName;
            set => _fileName = FileUtils.RemoveIllegalCharsFromFileName(value);
        }
        [BsonIgnore]
        [JsonIgnore]
        private string _fileName { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public double Version { get; set; }
        public AccessLevel AccessLevel { get; set; } = AccessLevel.Public;
        public AccountReference FileOwner { get; set; }
        public List<AccountReference> GrantedAccounts { get; set; } = new List<AccountReference>();
        
        public List<CdnFileReference> FileStoredAt { get; protected set; } = new List<CdnFileReference>();
        public string GetPath()
        {
            return Path.Combine(FileController.Instance.GetBaseDirectory(this), FileId);
        }

        public CdnFileReference GetRandomCdnFileReference()
        {
            if (FileStoredAt.Count == 0)
                return null;
            
            var random = new Random().Next(0, FileStoredAt.Count-1);
            return FileStoredAt[random];
        }
        
    }
}