using System;
using System.Collections.Generic;
using System.IO;
using Communication.Messages.File;
using Core.Data.Account;
using Core.Data.File;
using MongoDB.Driver;

namespace Communication.Data.File
{
    public class UploadedFileInfo : ICdnFile
    {
        public long Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public FileStream FileStream { get; set; }
        public AccountReference FileOwner { get; set; }
        public List<AccountReference> GrantedAccounts { get; set; } = new List<AccountReference>();
        public FileHeaderMessage CreateFileHeader()
        {
            var savedFile = FileController.Instance.Collection.FindSync(x => x.FileId.Equals(FileId)).FirstOrDefault();

            return new FileHeaderMessage()
            {
                FileId = FileId,
                FileName = FileName,
                ContentType = ContentType,
                Description = Description,
                Created = savedFile?.Created ?? DateTime.Now,
                LastModified = DateTime.Now,
                Version = savedFile?.Version + 0.1 ?? 1.0,
                AccessLevel = AccessLevel,
                FileOwner = FileOwner,
                GrantedAccounts = savedFile?.GrantedAccounts ?? GrantedAccounts
            };
        }
        
    }
}