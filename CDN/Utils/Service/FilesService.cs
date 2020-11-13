using System;
using System.IO;
using System.Threading.Tasks;
using CDN.Controllers.Models.File;
using Core.Data.File;
using Core.Utils;

namespace CDN.Utils.Service
{
    public static class FilesService
    {

        public static FileStream GetFileStream(SavedFile savedFile)
        {
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

        public static async Task<SavedFile> SaveFile(UploadedFile uploadedFile)
        {
            try
            {
                var savedFile = new SavedFile()
                {
                    FileId = FileIdGenerator.Instance.GenerateId(),
                    FileName = uploadedFile.FileName,
                    ContentType = uploadedFile.ContentType,
                    Description = uploadedFile.Description,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    Version = 1.0
                };
                
                var baseDirectory = FileController.Instance.GetPathForNewFile(uploadedFile);
                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);

                await using (var stream = new FileStream(Path.Combine(baseDirectory, savedFile.FileId), FileMode.Create))
                {
                    await uploadedFile.File.CopyToAsync(stream);
                }

                await FileController.Instance.Collection.InsertOneAsync(savedFile);
                
                return savedFile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
        
    }
}