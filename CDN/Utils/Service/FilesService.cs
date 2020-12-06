using System;
using System.IO;
using System.Threading.Tasks;
using Commons;
using Core.Data.File;

namespace CDN.Utils.Service
{
    public static class FilesService
    {

        public static FileStream GetFileStream(SavedFile savedFile)
        {
            Logger.Debug($"Path: {savedFile.GetPath()}");
            return File.OpenRead(savedFile.GetPath());
        }

        public static async Task<string> GetFileContent(SavedFile savedFile)
        {
            return await File.ReadAllTextAsync(savedFile.GetPath());
        }

        public static async Task<byte[]> GetFileBytes(SavedFile savedFile)
        {
            return await File.ReadAllBytesAsync(savedFile.GetPath());
        }

        public static async Task<bool> SaveFile(SavedFile savedFile, string tempFile)
        {
            try
            {
                
                var baseDirectory = FileController.Instance.GetPathForNewFile(savedFile);
                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                File.Copy(tempFile, Path.Combine(baseDirectory, savedFile.FileId), false);

                
                await FileController.Instance.Collection.InsertOneAsync(savedFile);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //TODO handle file already exists possibility
                return false;
            }

        }
        
    }
}