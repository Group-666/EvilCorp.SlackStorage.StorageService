using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes
{
    public class DocumentParser
    {
        public static Document Parse(JObject json)
        {
            var document = (Object)json["document"] ?? throw new ArgumentException("The document is not found in json object.");

            return new Document(document);
        }
    }
}
