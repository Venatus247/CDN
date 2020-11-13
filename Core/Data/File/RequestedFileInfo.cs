namespace Core.Data.File
{
    public class RequestedFileInfo : ICdnFile
    {
        public long Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}