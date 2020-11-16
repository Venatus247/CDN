using System.IO;
using System.Threading.Tasks;
using API.Controllers.Models.File;

namespace API.Utils.Service
{
    public static class FilesService
    {

        public static async Task<string> SaveTempFile(UploadedFile uploadedFile)
        {
            var path = Path.GetTempFileName();
            await using var stream = new FileStream(path, FileMode.Create);
            await uploadedFile.File.CopyToAsync(stream);
            return path;
        }

        public static async Task<FileStream> SaveTempFileAndGetFileStream(UploadedFile uploadedFile)
        {
            var path = await SaveTempFile(uploadedFile);
            return File.OpenRead(path);
        }
        
    }
}