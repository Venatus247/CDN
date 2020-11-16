﻿using Core.Controller;
using Core.Data.File;
using Core.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Data.Cdn
{
    public class CdnReference : IIdentified
    {
        [BsonId(IdGenerator = typeof(BsonIncrementGenerator<CdnReference>))]
        public long Id { get; set; }
        public string CdnId { get; set; }
        public string CdnUrlAddress { get; set; }

        public string GetFileUrl(ICdnFile cdnFile)
        {
            return $"{CdnUrlAddress}/file/get/{cdnFile.FileId}/{cdnFile.FileName}";
        }
        
    }
}