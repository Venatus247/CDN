using System.IO;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Controller;
using MongoDB.Driver;

namespace Core.Data.File
{
    public class FileController : DatabaseController<FileController, SavedFile>
    {
        
        private FileStorageConfiguration FileStorageConfiguration { get; }

        protected FileController()
        {
            FileStorageConfiguration = BackendServer.Instance.ConfigLoader.GetConfiguration<FileStorageConfiguration>();
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
        
    }
}