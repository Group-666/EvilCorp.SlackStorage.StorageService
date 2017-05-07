﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DomainTypes;
using DataAccess;
using MongoDB.Driver;
using DomainTypes.Contracts;
using MongoDB.Bson;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvilCorp.SlackStorage.StorageService.WebHost.Controllers
{

    [Route("api/[controller]")]
    public class StorageController : Controller
    {
        private readonly ILogger _logger;
        private readonly IStorageRepository _dataStoreRepo;

        public StorageController()
        {
            _logger = ConsoleFactory.CreateLogger();

            //Want to prevent hardcoding of the mongoClient IP. 
            _dataStoreRepo = new StorageRepository(new MongoClient("mongodb://127.0.0.1:32768/"));
        }
        // GET: api/storage/<userId>
        [HttpGet("{userId}")]
        public JsonResult Get(String userId)
        {
            List<DataStore> dataStores = _dataStoreRepo.GetAll(userId);
            _logger.Log("StorageController:Get - {userId}: getting all datastores for user" + userId, LogLevel.Trace);
            return Json(dataStores);
        }
        [HttpGet("{userId}/{dataStoreId}")]
        public JsonResult Get(string userId, string dataStoreId)
        {
            DataStore dataStore;
            try
            {
                dataStore = _dataStoreRepo.GetOne(userId, dataStoreId);
                _logger.Log("StorageController:Get - {userId}/{dataStoreId}: getting datastore " + dataStoreId, LogLevel.Trace);
            }
            catch (KeyNotFoundException kyfe)
            {
                //What should I return? An Empty DataStore? Ideally a string but I can't do that.
                _logger.Log("StorageController:Get - {userId}/{dataStoreId}: A datastore with that id was not found for that user: " + kyfe, LogLevel.Error);
                return null;
            }
            

            return Json(dataStore);
        }

        // POST api/storage
        [HttpPost("{userId}")]
        public JsonResult Post([FromBody]JObject json, string userId)
        {
            try
            {
                var dataStore = DataStoreParser.Parse(json);
                dataStore.UserId = userId;

                //Creates a datastore for the user specified.
                var dataStoreId = _dataStoreRepo.Create(dataStore);
                _logger.Log("StorageController:Post {userId} :  datastore for user: " + dataStore.UserId + " created with an id of: " + dataStoreId, LogLevel.Trace);
                
                return Json(dataStoreId);

            }
            catch (Exception ex)
            {
                _logger.Log("StorageController:Post {userId} : Error in trying to create a datastore. Message: " + ex.Message, LogLevel.Error);
                return Json(ex.Message);
            }
        }
        [HttpDelete("{userId}/{dataStoreId}")]
        public JsonResult DeleteOneDataStore(string userId, string dataStoreId)
        {
            try
            {
                var message = _dataStoreRepo.DeleteOneDataStore(userId, dataStoreId);
                _logger.Log("StorageController:Delete {userId}/{dataStoreId} - deleting datastore " + dataStoreId, LogLevel.Trace);
                return Json(message);
            }
            catch (Exception except)
            {
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: "+ except.Message, LogLevel.Critical);
                return Json(except.Message);
            }
            
        }
        [HttpDelete("{userId}")]
        public JsonResult DeleteAllDataStores(string userId)
        {
            try
            {
                var message = _dataStoreRepo.DeleteAllDataStores(userId);
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: deleting all datastores for user" + userId, LogLevel.Trace);
                return Json(message);
            }
            catch (Exception except)
            {
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: " + except.Message, LogLevel.Critical);
                return Json(except.Message);
            }
           
        }
        
     


 
    }
}
