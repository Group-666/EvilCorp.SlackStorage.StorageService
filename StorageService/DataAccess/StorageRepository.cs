using System;
using DomainTypes;
using MongoDB.Driver;
using DomainTypes.Contracts;

namespace DataAccess
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IMongoClient _client;
        private readonly ILogger _logger;
        private readonly String database = "StorageService";

        public StorageRepository(IMongoClient client)
        {
            _client = client;
            _logger = ConsoleLoggerFactory.CreateConsoleLogger();
        }
        public String Create(DataStore dataStore)
        {
            try
            {
                var db = _client.GetDatabase(database);

                var dataStoreId = dataStore.UserId + "_" + dataStore.DataStoreName;
                //For some reason GetCollection wasn't creating a collection if one didn't exist before,
                //hence the create collection here. Will throw an exception if a database with that name already exists.
                db.CreateCollection(dataStoreId);

                _logger.Log("Collection for user " + dataStore.UserId + " created called. " + dataStoreId, LogLevel.Information);

                return dataStoreId;

            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }
    }
}
       
