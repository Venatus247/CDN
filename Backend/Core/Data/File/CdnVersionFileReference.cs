using Core.Controller;

namespace Core.Data.File
{
    public class CdnVersionFileReference : IMapped
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public double Version { get; set; }
    }
}