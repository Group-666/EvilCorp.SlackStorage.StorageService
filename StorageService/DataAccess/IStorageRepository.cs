using DomainTypes;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public interface IStorageRepository
    {
        String Create(DataStore dataStore);
        List<DataStore> GetAll(string userId);
        DataStore GetOne(string userId, string dataStoreId);
        String Insert(BsonDocument document, String dataStoreId);

        string DeleteOneDataStore(string userId, string dataStoreId);
        string DeleteAllDataStores(string userId);
    }
}
