using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Commons.Configuration
{
    public class ConfigLoader
    {
        public static string DefaultConfigurationDirectory => "cfg";
        public static string DefaultConfigurationExtension => "json";
        public static Encoding DefaultConfigurationEncoding => Encoding.UTF8;

        private readonly string _configDirectory;
        private readonly string _configExtension;
        private Encoding _configEncoding;

        #region Constructors

        public ConfigLoader() : this(DefaultConfigurationDirectory, DefaultConfigurationExtension, DefaultConfigurationEncoding) {}
        
        public ConfigLoader(string configDirectory) : this(configDirectory, DefaultConfigurationExtension, DefaultConfigurationEncoding) {}

        public ConfigLoader(string configDirectory, string configExtension) : this(configDirectory, configExtension, DefaultConfigurationEncoding) {}

        public ConfigLoader(string configDirectory, string configExtension, Encoding configEncoding)
        {
            _configDirectory = configDirectory;
            _configExtension = configExtension;
            _configEncoding = configEncoding;
        }
        
        #endregion

        #region Private Methods

        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private string GetConfigName<T>() where T : IConfiguration, new()
        {
            return new T().ConfigurationName;
        }

        private string GetAbsoluteConfigPath<T>() where T : IConfiguration, new()
        {
            var absolutePath = Path.Combine(_configDirectory, GetConfigName<T>());
            Logger.Debug($"{Path.GetFullPath($"{absolutePath}.{_configExtension}")}");
            return $"{absolutePath}.{_configExtension}";
        }
        
        #endregion

        #region Public Methods

        public T GetConfiguration<T>() where T : IConfiguration, new()
        {
            EnsureDirectory(_configDirectory);

            var configPath = GetAbsoluteConfigPath<T>();

            if (File.Exists(configPath))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(configPath, _configEncoding));

            var config = new T();
            config.LoadDefault();
            
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented), _configEncoding);

            return config;
        }

        public void UpdateConfiguration<T>(T config) where T : IConfiguration, new()
        {
            RemoveConfiguration<T>();

            var configPath = GetAbsoluteConfigPath<T>();
            
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented), _configEncoding);

        }
        
        public void RemoveConfiguration<T>() where T : IConfiguration, new()
        {
            EnsureDirectory(_configDirectory);

            var configurationPath = GetAbsoluteConfigPath<T>();

            if (File.Exists(configurationPath))
                File.Delete(configurationPath);
        }

        
        #endregion
        
    }
}