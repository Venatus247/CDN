using System;
using System.Linq;
using API.Controllers;
using Commons;
using Commons.Configuration;
using Core.Configuration;
using Core.Controller;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Core
{
    public class BackendServer : Singleton<BackendServer>
    {
        public const double Version = 1.01;
        public ConfigLoader ConfigLoader { get; private set; }
        public IMongoDatabase Database { get; private set; }

        protected BackendServer()
        {
            ConfigLoader = new ConfigLoader();
        }
        
        public void OnStartup()
        {
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                    x.GetTypes()).Where(x => !x.IsAbstract && typeof(IMapped).IsAssignableFrom(x)).ToList()
                .ForEach(t => BsonClassMap.LookupClassMap(t));
            
            OpenDatabase();
        }
        
        public void OnStop()
        {
            
        }

        #region Private Methods

        private void OpenDatabase()
        {
            var config = ConfigLoader.GetConfiguration<DatabaseConfiguration>();
            
            Logger.Info($"Database: Connecting to {config.Host}:{config.Port}");
            
            var client = new MongoClient($"mongodb://{config.Host}:{config.Port}");
            Database = client.GetDatabase(config.Database);
            
            Logger.Info($"Database: Using Database '{config.Database}'");
            
        }

        #endregion
        
    }
}