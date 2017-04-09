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
        public void create(DataStore dataStore)
        {
            try
            {
                var db = _client.GetDatabase("StorageService");
                
                //We're going to want to create a unique collection. Possibly with the accountId. 
                db.CreateCollection(dataStore.DataStoreName);
                
                Console.WriteLine(db);
            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }
    }
}
