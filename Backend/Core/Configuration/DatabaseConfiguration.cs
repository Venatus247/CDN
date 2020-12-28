using Commons.Configuration;

namespace Core.Configuration
{
    public class DatabaseConfiguration : IConfiguration
    {
        public string ConfigurationName { get; } = "Database";
            
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }

        public void LoadDefault()
        {
            Host = "127.0.0.1";
            Port = 27017;
            Database = "StoredData";
        }
        
    }
}