using Commons;
using MongoDB.Driver;

namespace Core.Controller
{
    public abstract class DatabaseController<TController, T> : IdentifiedController<TController, T>
        where T : class, IIdentified where TController : DatabaseController<TController, T>
    {
        
        public IMongoCollection<T> Collection => BackendServer.Instance.Database.GetCollection<T>(typeof(T).Name);

        public override void OnStartup()
        {
            Logger.Info($"Starting up {GetType().Name}");
            base.OnStartup();

            Collection.FindSync(entity => true).ToList(). //ensure all structures in db are up-to-date DEPRECATED!
                ForEach(entity => Collection.ReplaceOne(found => found.Id == entity.Id, entity));
        }
    }
}