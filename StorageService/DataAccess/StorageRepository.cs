using System;
using DomainTypes;
using MongoDB.Driver;
using DomainTypes.Contracts;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace DataAccess
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IMongoClient _client;
        private readonly ILogger _logger;
        private readonly string database = "StorageService";
        private readonly IMongoDatabase _db;
        private readonly string _metaDataCollection = "collectionMetaData";

        public StorageRepository(IMongoClient client)
        {
            _client = client;
            _logger = ConsoleFactory.CreateLogger();

            _db = _client.GetDatabase(database);
        }

        public List<DataStore> GetAll(string userId)
        {
            var dataStores = GetDataStoresForAccount(userId);
            _logger.Log("StorageRepository - GetAll: getting all datastores for " + userId, LogLevel.Trace);
            return dataStores;

        }
        public DataStore GetOne(string userId, string dataStoreId)
        {
            var dataStores = GetDataStoresForAccount(userId);
            _logger.Log("Getting all datastores for an account and looping through it to find the one we want. DataStoreId" + dataStoreId , LogLevel.Trace);
            //Ideally don't loop through datastores but just get it from db directly.
            foreach (DataStore ds in dataStores)
            {
                if (ds.DataStoreId.Equals(dataStoreId))
                {
                    _logger.Log("StorageRepository - GetOne: Found the datastore we where looking for.", LogLevel.Trace);
                    return ds;
                }
                
            }
            _logger.Log("StorageRepository - GetOne: Oh no! datastore with id " + dataStoreId + " Does not exist", LogLevel.Error);
            throw new KeyNotFoundException();
        }

        public string Create(DataStore dataStore)
        {
            try
            {
                //** MetaData preparation **//
                Account account = new Account(dataStore.UserId);
                
                var collection = _db.GetCollection<Account>(_metaDataCollection);

                //TODO Instead of looking to see if the user has an account, should check if there are any datastores in the list.
                var doesUserExist = collection.Find(a => a.AccountId == dataStore.UserId).ToList();
                _logger.Log("StorageRepository - Create:  Has user created datastore before?: " + doesUserExist.Count, LogLevel.Trace);
                
                var dataStoreId = dataStore.UserId + "_" + dataStore.DataStoreName;
                //removing spaces.
                dataStoreId = Regex.Replace(dataStoreId, @"\s+", "");
                _logger.Log("StorageRepository - Create: dataStoreId " + dataStoreId, LogLevel.Trace);
                dataStore.DataStoreId = dataStoreId;
                
                //** Create the actual collection **//
                _db.CreateCollection(dataStoreId);
                _logger.Log("StorageRepository - Create: Collection for user " + dataStore.UserId + " created called. " + dataStoreId, LogLevel.Information);
                
                //This user has not yet created a datastore.
                if (doesUserExist.Count == 0)
                {
                    CreateNewAccountWithMetaData(dataStore,collection,account);
                    _logger.Log("StorageRepository - Create: New Account and associated datastore created in metaData table", LogLevel.Information);

                    return dataStoreId;
                }
                //User has created a datastore before. Let's add the new one to their collection. 
                else if (doesUserExist.Count > 0)
                {
                    AddToAccountDataStoresMetaData(dataStore,collection);
                    _logger.Log("StorageRepository - Create: Adding additional datastore to an existing account", LogLevel.Trace);

                    return dataStore.DataStoreId;
                }

                return dataStore.DataStoreId;

            }
            catch (Exception exception)
            {
                _logger.Log("StorageRepository - Create: There was a problem creating the data store  " + exception.Message, LogLevel.Critical);
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }

        public string Insert(BsonDocument document, string dataStoreId)
        {

            //This sounds like it should go in the document repository...
            //Future me, do your stuff!
            //Shut up past me, that was your job.

            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            collection.InsertOne(document);
            var documentId = document["_id"].ToString();

            _logger.Log("StorageRepository - Insert: Inserted document into " + dataStoreId, LogLevel.Trace);
            _logger.Log("StorageRepository - Insert: ID of inserted document " + documentId, LogLevel.Trace);

            return documentId;
           
        }
        public string DeleteOneDataStore(string userId, string dataStoreId)
        {
            
           _db.DropCollection(dataStoreId);
            _logger.Log("StorageRepository - DeleteOneDataStore:  Anhillating datastore " + dataStoreId, LogLevel.Information);
           var message = "Store with Id: " + dataStoreId + " has been removed";
           

            /**** Updating MetaData ****/
            //Filter is to find the user within the meta data collection.
            //Update finds a specific datastore within the datastore array.

            var collection = _db.GetCollection<Account>(_metaDataCollection);
            var filter = Builders<Account>.Filter.Eq(a => a.AccountId, userId);
            _logger.Log("StorageRepository - DeleteOneDataStore: filter for finding user from metadata " + filter, LogLevel.Trace);

            //We want a PullFilter rather than just a pull to pull (remove) a single element in an array.
            var update = Builders<Account>.Update.PullFilter("DataStores",
                Builders<DataStore>.Filter.Eq("DataStoreId", dataStoreId));

            _logger.Log("StorageRepository - DeleteOneDataStore: Filter for updating the datastores list in metadata collection" + update, LogLevel.Trace);
            collection.FindOneAndUpdate(filter, update);
            _logger.Log("StorageRepository - DeleteOneDataStore: Attempted to update user " + userId + " in metadata. Not sure if succesfull", LogLevel.Information);
            //Only return this message if it's been done succesfully.
            return message;
        }

        public string DeleteAllDataStores(string userId)
        {
            //First find all datastores for a user.
            var dataStores = GetDataStoresForAccount(userId);
            _logger.Log("StorageRepository - DeleteAllDataStores: getting all datastores for a particular user", LogLevel.Trace);
            //Remove each collection. Has problems if datastoreId is null. 
            foreach (DataStore datastore in dataStores) {
                _db.DropCollection(datastore.DataStoreId);
                _logger.Log("StorageRepository - DeleteAllDataStores: Deleting datastore Id " + datastore.DataStoreId, LogLevel.Trace);
            }
               
            


            //Just remove the entire user document instead of just the datastores list. 
            var collection = _db.GetCollection<Account>(_metaDataCollection);
            
            collection.DeleteOne(a => a.AccountId == userId);
            _logger.Log("StorageRepository - DeleteAllDataStores: deleting datastore from metadata", LogLevel.Trace);

            //Should we come up with a better message? Or infact even just not return anything? Naaaah.
            return "gooodbye datastores";

        }

        //********** Private Methods ************//
        private List<DataStore> GetDataStoresForAccount(string userId)
        {
            List<DataStore> dataStores = new List<DataStore>();
            var collection = _db.GetCollection<Account>("collectionMetaData");
            var list = collection.Find(m => m.AccountId == userId).ToList();
            _logger.Log("StorageRepository - GetDataStoresForAccount: Found list of collections belonging to user in metadata", LogLevel.Trace);
            //There should only be one account in the list anyway
            if (list.Count != 0) {
                var account = list[0];
                dataStores = account.DataStores;

                //TODO still need to add size and number of documents.

                return dataStores;
            }
            else
            {
                _logger.Log("StorageRepository - GetDataStoresForAccount: No datastores for that user found in metadataCollection", LogLevel.Error);
                return new List<DataStore>();
            }
          
        }
        private void AddToAccountDataStoresMetaData(DataStore dataStore, IMongoCollection<Account> collection)
        {

            var filter = Builders<Account>.Filter.Eq(a => a.AccountId, dataStore.UserId);
            _logger.Log("StorageRepository - AddToAccountDataStoresMetaData: Filter to find an account that matches the userId of a datastore to an accountId " + filter, LogLevel.Trace);
            var update = Builders<Account>.Update.Push("DataStores", dataStore);
            _logger.Log("StorageRepository - AddToAccountDataStoresMetaData: Pushed datastore onto the datastore list for user with Id " + dataStore.UserId, LogLevel.Information);
            collection.FindOneAndUpdate(filter, update);
        }

        private void CreateNewAccountWithMetaData(DataStore dataStore, IMongoCollection<Account> collection,Account account)
        {
            //Create datastore list and add the datastore to it. Then append the list to the account before inserting.
            List<DataStore> dataStores = new List<DataStore>
            {
                dataStore
            };
            account.DataStores = dataStores;
            collection.InsertOne(account);
            _logger.Log("StorageRepository - CreateNewAccountWithMetaData: just inserteded the new account into metadata collection. userId " + account.AccountId, LogLevel.Information);

        }

    
    }
}
       
