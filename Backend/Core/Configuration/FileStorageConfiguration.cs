using System;
using Commons.Configuration;

namespace Core.Configuration
{
    public class FileStorageConfiguration : IConfiguration
    {
        public string ConfigurationName { get; } = "FileStorage";
        
        public string CdnId { get; set; }
        public string CdnToken { get; set; }
        public string CdnUrlAddress { get; set; }
        public string BaseDirectory { get; set; }

        public void LoadDefault()
        {
            CdnId = Guid.NewGuid().ToString();
            BaseDirectory = "uploads";
        }
    }
}