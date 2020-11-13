using System.Linq;
using Core.Controller;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Core.Serialization
{
    public class BsonIncrementGenerator<T> : IIdGenerator where T : IIdentified
    {
        public object GenerateId(object container, object document)
        {
            var collection = (IMongoCollection<T>) container;
            var last = collection.FindSync(FilterDefinition<T>.Empty).ToList().OrderByDescending(item => item.Id).FirstOrDefault();
            var id = last == null ? 1 : (long) last.ToBsonDocument()["_id"] + 1;
            return id;
        }

        public bool IsEmpty(object id)
        {
            if (id != null)
                return (long) id == 0;
            return true;
        }
    }
}