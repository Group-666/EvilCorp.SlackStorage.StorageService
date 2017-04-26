using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes
{
    
    public class Document
    {
        [BsonIgnoreIfDefault]
        public Object Id { get; set; }
        public BsonDocument Doc { get; set; }

        public Document(BsonDocument document)
        {
            Doc = document;
        }
    }
}
