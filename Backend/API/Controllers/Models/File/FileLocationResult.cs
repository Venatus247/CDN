namespace API.Controllers.Models.File
{
    public class FileLocationResult
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string UrlAddress { get; set; }

        public FileLocationResult(string urlAddress)
        {
            UrlAddress = urlAddress;
        }
        public FileLocationResult(string fileId, string fileName, string urlAddress)
        {
            FileId = fileId;
            FileName = fileName;
            UrlAddress = urlAddress;
        }
    }
}