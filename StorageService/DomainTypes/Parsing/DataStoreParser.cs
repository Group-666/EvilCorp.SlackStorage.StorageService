using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes
{
    public class DataStoreParser
    {
        public static DataStore Parse(JObject json)
        {
            var dataStoreName = (string)json["dataStoreName"] ?? throw new ArgumentException("The dataStoreName not found in json object.");
            //var userId = (string)json["userId"] ?? throw new ArgumentException("The userId was not found in json object.");

            return new DataStore(dataStoreName);
        }
    }
}
