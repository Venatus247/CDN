﻿using System.IO;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Controller;
using MongoDB.Driver;

namespace Core.Data.File
{
    public class FileController : DatabaseController<FileController, SavedFile>
    {

        private FileStorageConfiguration FileStorageConfiguration =>
            _fileStorageConfiguration ??= BackendServer.Instance.ConfigLoader.GetConfiguration<FileStorageConfiguration>();

        private FileStorageConfiguration _fileStorageConfiguration;

        protected FileController()
        {
            
        }

        public async Task<SavedFile> TryGetFile(ICdnFile fileInfo)
        {
            SavedFile savedFile;
            var foundFiles = await Collection.FindAsync(x => x.FileId.Equals(fileInfo.FileId));

            lock (foundFiles)
            {
                savedFile = foundFiles.FirstOrDefault();
            }

            return savedFile;
        }

        public string GetPathForNewFile(ICdnFile cdnFile)
        {
            return Path.GetFullPath(FileStorageConfiguration.BaseDirectory);
        }

        public string GetBaseDirectory(ICdnFile cdnFile)
        {
            return Path.GetFullPath(FileStorageConfiguration.BaseDirectory);
        }

        public string GetCdnId()
        {
            return FileStorageConfiguration.CdnId;
        }

        public string GetCdnToken()
        {
            return FileStorageConfiguration.CdnToken;
        }

        public string GetCdnUrlAddress()
        {
            return FileStorageConfiguration.CdnUrlAddress;
        }
        
    }
}