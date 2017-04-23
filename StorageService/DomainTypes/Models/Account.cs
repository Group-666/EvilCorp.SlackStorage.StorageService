using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;


namespace DomainTypes
{
    public class Account
    {
        [BsonIgnoreIfDefault]
        public Object Id { get; set; }
        public String AccountId { get; set; }
        public List<DataStore> DataStores { get; set; }

        public Account(String id, List<DataStore> dataStores)
        {
            AccountId = id;
            DataStores = dataStores;
        }
        /**
        public Account(Object _id, String accountId, List<DataStore> dataStores)
        {
            Id = _id;
            AccountId = accountId;
            DataStores = dataStores;
        }
        **/
        public Account(String accountId)
        {
            AccountId = accountId;
        }
     
    }
}
