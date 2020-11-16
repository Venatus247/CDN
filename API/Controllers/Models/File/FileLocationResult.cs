namespace API.Controllers.Models.File
{
    public class FileLocationResult
    {
        public string UrlAddress { get; set; }

        public FileLocationResult(string urlAddress)
        {
            UrlAddress = urlAddress;
        }
    }
}