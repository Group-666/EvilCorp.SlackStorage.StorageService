using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes
{
    
    public class Document
    {
        [BsonIgnoreIfDefault]
        public Object Id { get; set; }
        public Object Doc { get; set; }

        public Document(Object document)
        {
            Doc = document;
        }
    }
}
