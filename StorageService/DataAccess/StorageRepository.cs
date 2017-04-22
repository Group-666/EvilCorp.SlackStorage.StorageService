﻿using System;
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
            

            var collection = _db.GetCollection<Account>("collectionMetaData");

            var list = collection.Find(m => m.AccountId == userId).ToList();

            //There should only be one account in the list anyway
            var account = list[0];
            dataStores = account.DataStores;

            //TODO still need to add size and number of documents.

            return dataStores;

        }
        public String Create(DataStore dataStore)
        {
            try
            {
                //DataStore dataStore = new DataStore("samualTarly", "23481");               
                //dataStores.Add(dataStore);
                // Account account = new Account("23481", dataStores);
                // collection.InsertOne(account);

                Account account = new Account(dataStore.UserId);
                var collection = _db.GetCollection<Account>("collectionMetaData");
                var doesUserExist = collection.Find(a => a.AccountId == dataStore.UserId).ToList();

                //Clearly no user exists with that name
                if (doesUserExist.Count == 0)
                {
                    var dataStoreId = dataStore.UserId + "_" + dataStore.DataStoreName;
                    dataStore.DataStoreId = dataStoreId;
                    //For some reason GetCollection wasn't creating a collection if one didn't exist before,
                    //hence the create collection here. Will throw an exception if a database with that name already exists.
                    _db.CreateCollection(dataStoreId);
                    

                    //Create info about when a database was created. 
                    CreateCollectionWithMetaData(dataStore,collection);
                    _logger.Log("Collection for user " + dataStore.UserId + " created called. " + dataStoreId, LogLevel.Information);

                    return dataStoreId;
                }
                //I think we may have a user with that name. 
                else if (doesUserExist.Count > 0)
                {            
                    return "0";
                }

                return "0";
              

             

            }
            catch (Exception exception)
            {
                throw new InvalidProgramException("There was a problem creating the data store", exception);
            }
        }
        private void CreateCollectionWithMetaData(DataStore dataStore, IMongoCollection<Account> collection)
        {

            
            Account account = new Account(dataStore.UserId);
            List<DataStore> dataStores = new List<DataStore>
            {
                dataStore
            };
            account.DataStores = dataStores;

            //var collection = _db.GetCollection<Account>("collectionMetaData");
        
            collection.InsertOne(account);
            
        
        }

    }
}
       
