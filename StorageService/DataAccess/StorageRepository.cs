using System;
using DomainTypes;
using MongoDB.Driver;
using DomainTypes.Contracts;
using System.Collections.Generic;
using DomainTypes.Models;

namespace DataAccess
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IMongoClient _client;
        private readonly ILogger _logger;
        private readonly String database = "StorageService";
        private readonly IMongoDatabase _db;

        public StorageRepository(IMongoClient client)
        {
            _client = client;
            _logger = ConsoleLoggerFactory.CreateConsoleLogger();

            _db = _client.GetDatabase(database);
        }

        public List<DataStore> GetAll(string userId)
        {
            List<DataStore> dataStores = new List<DataStore>();
           
            


            return dataStores;

        }
        public String Create(DataStore dataStore)
        {
            try
            {
                

                var dataStoreId = dataStore.UserId + "_" + dataStore.DataStoreName;
                //For some reason GetCollection wasn't creating a collection if one didn't exist before,
                //hence the create collection here. Will throw an exception if a database with that name already exists.
                _db.CreateCollection(dataStoreId);
                
                //Create info about when a database was created. 
                CreateCollectionWithMetaData(dataStoreId);
                _logger.Log("Collection for user " + dataStore.UserId + " created called. " + dataStoreId, LogLevel.Information);

                return dataStoreId;

            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }
        private void CreateCollectionWithMetaData(string collectionId)
        {

            //TODO Handle errors.

            var collection = _db.GetCollection<MetaData>("collectionMetaData");
        
            var currentTime = DateTime.Now;
            var metaData = new MetaData(collectionId,currentTime);
            collection.InsertOne(metaData);
            
        
        }

    }
}
       
