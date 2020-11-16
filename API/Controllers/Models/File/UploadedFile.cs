using System.Text.Json.Serialization;
using Core.Data.File;
using Core.Serialization;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Controllers.Models.File
{
    public class UploadedFile : ICdnFile
    {
        [BsonId(IdGenerator = typeof(BsonIncrementGenerator<SavedFile>))]
        public long Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public IFormFile File { get; set; }
    }
}