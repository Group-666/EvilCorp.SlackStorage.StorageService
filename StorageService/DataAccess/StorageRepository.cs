using System;
using DomainTypes;
using MongoDB.Driver;

namespace DataAccess
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IMongoClient _client;

        public StorageRepository(IMongoClient client)
        {
            _client = client;

            
        }
        public String create(DataStore dataStore)
        {
            try
            {
                var db = _client.GetDatabase("StorageService");

                var dataStoreId = dataStore.DataStoreName + "_" + dataStore.UserId;
                db.CreateCollection(dataStoreId);

                return dataStoreId;
                
            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }
    }
}
