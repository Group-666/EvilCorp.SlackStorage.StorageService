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
            throw new NotImplementedException();
        }
    }
}
