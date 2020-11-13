using System.Text.Json.Serialization;
using Commons.Utils;
using Core.Controller;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Data.File
{
    public interface ICdnFile : IIdentified
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}