using Commons.Configuration;

namespace Core.Configuration
{
    public class FileStorageConfiguration : IConfiguration
    {
        public string ConfigurationName { get; } = "FileStorage";
        
        public string CdnId { get; set; }
        public string BaseDirectory { get; set; }

        public void LoadDefault()
        {
            CdnId = null;
            BaseDirectory = "uploads";
        }
    }
}