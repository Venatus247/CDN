using Newtonsoft.Json;

namespace Commons.Configuration
{
    public interface IConfiguration
    {
        [JsonIgnore]
        string ConfigurationName { get; }

        public virtual void LoadDefault()
        {
        }

    }
}