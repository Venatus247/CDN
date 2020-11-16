using Core.Controller;

namespace Core.Data.File
{
    public interface ICdnFile : IIdentified
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}