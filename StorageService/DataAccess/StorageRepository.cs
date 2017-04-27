using System;
using DomainTypes;
using MongoDB.Driver;
using DomainTypes.Contracts;
using System.Collections.Generic;
using MongoDB.Bson;

namespace DataAccess
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IMongoClient _client;
        private readonly ILogger _logger;
        private readonly string database = "StorageService";
        private readonly IMongoDatabase _db;

        public StorageRepository(IMongoClient client)
        {
            _client = client;
            _logger = ConsoleFactory.CreateConsoleLogger();

            _db = _client.GetDatabase(database);
        }

        public List<DataStore> GetAll(string userId)
        {
            var dataStores = GetDataStoresForAccount(userId);

            return dataStores;

        }
        public DataStore GetOne(string userId, string dataStoreId)
        {
            var dataStores = GetDataStoresForAccount(userId);
            
            //Ideally don't loop through datastores but just get it from db directly.
            foreach (DataStore ds in dataStores)
            {
                if (ds.DataStoreId.Equals(dataStoreId))
                {
                    return ds;
                }
                
            }
            throw new KeyNotFoundException();
        }

        public string Create(DataStore dataStore)
        {
            try
            {
                //** MetaData preparation **//
                Account account = new Account(dataStore.UserId);
                var collection = _db.GetCollection<Account>("collectionMetaData");
                var doesUserExist = collection.Find(a => a.AccountId == dataStore.UserId).ToList();
                var dataStoreId = dataStore.UserId + "_" + dataStore.DataStoreName;
                dataStore.DataStoreId = dataStoreId;
                _logger.Log("Collection for user " + dataStore.UserId + " created called. " + dataStoreId, LogLevel.Information);
                
                //** Create the actual collection **//
                _db.CreateCollection(dataStoreId);

                //This user has not yet created a datastore.
                if (doesUserExist.Count == 0)
                {
                    
                    CreateNewAccountWithMetaData(dataStore,collection,account);
                    _logger.Log("New Account and associated datastore created in metaData table", LogLevel.Information);

                    return dataStoreId;
                }
                //User has created a datastore before. Let's add the new one to their collection. 
                else if (doesUserExist.Count > 0)
                {
                    AddToAccountDataStoresMetaData(dataStore,collection);
                    _logger.Log("Adding additional datastore to an existing account", LogLevel.Information);

                    return dataStore.DataStoreId;
                }

                return dataStore.DataStoreId;

            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }

        public string Insert(BsonDocument document, string dataStoreId)
        {

            //This sounds like it should go in the document repository...
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            collection.InsertOne(document);
            var documentId = document["_id"].ToString();

            _logger.Log("Inserted document into " + dataStoreId, LogLevel.Information);
            _logger.Log("ID of inserted document " + documentId, LogLevel.Information);

            return documentId;
           
        }
        public string DeleteOneDataStore(string userId, string dataStoreId)
        {
            
           _db.DropCollection(dataStoreId);
           var message = "Store with Id: " + dataStoreId + " has been removed";
           _logger.Log(message, LogLevel.Information);

            /**** Updating MetaData ****/
            //Filter is to find the user within the meta data collection.
            //Update finds a specific datastore within the datastore array.

            var collection = _db.GetCollection<Account>("collectionMetaData");
            var filter = Builders<Account>.Filter.Eq(a => a.AccountId, userId);

            //We want a PullFilter rather than just a pull to pull (remove) a single element in an array.
            var update = Builders<Account>.Update.PullFilter("DataStores",
                Builders<DataStore>.Filter.Eq("DataStoreId", dataStoreId));

            collection.FindOneAndUpdate(filter, update);
            //Only return this message if it's been done succesfully.
            return message;
        }

        public string DeleteAllDataStores(string userId)
        {
            throw new NotImplementedException();
        }

        //********** Private Methods ************//
        private List<DataStore> GetDataStoresForAccount(string userId)
        {
            List<DataStore> dataStores = new List<DataStore>();
            var collection = _db.GetCollection<Account>("collectionMetaData");
            var list = collection.Find(m => m.AccountId == userId).ToList();
            //There should only be one account in the list anyway
            var account = list[0];
            dataStores = account.DataStores;

            //TODO still need to add size and number of documents.

            return dataStores;
        }
        private void AddToAccountDataStoresMetaData(DataStore dataStore, IMongoCollection<Account> collection)
        {

            var filter = Builders<Account>.Filter.Eq(a => a.AccountId, dataStore.UserId);
            var update = Builders<Account>.Update.Push("DataStores", dataStore);
            //var delete = Builders<Account>.Update.
            collection.FindOneAndUpdate(filter, update);
        }

        private void CreateNewAccountWithMetaData(DataStore dataStore, IMongoCollection<Account> collection,Account account)
        {
            
            List<DataStore> dataStores = new List<DataStore>
            {
                dataStore
            };
            account.DataStores = dataStores;
            collection.InsertOne(account);

        }

    
    }
}
       
