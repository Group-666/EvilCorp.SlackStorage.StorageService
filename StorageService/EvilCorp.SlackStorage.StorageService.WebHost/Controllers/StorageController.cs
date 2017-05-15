using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DomainTypes;
using DataAccess;
using MongoDB.Driver;
using DomainTypes.Contracts;
using MongoDB.Bson;
using Newtonsoft.Json;

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
            _dataStoreRepo = new StorageRepository(new MongoClient("mongodb://localhost:27017/"));
        }

        // GET: api/storage/<userId>
        [HttpGet("{userId}")]
        public IActionResult Get(string userId)
        {
            List<DataStore> dataStores = _dataStoreRepo.GetAll(userId);
            var count = dataStores.Count;
            _logger.Log("StorageController:Get - {userId}: getting all datastores for user" + userId, LogLevel.Trace);

            if (dataStores.Count != 0)
            {
                var json = JsonConvert.SerializeObject(new
                {
                    result = dataStores
                });

                return Ok(JObject.Parse(json));
            }
            else
            {
                _logger.Log("StorageController:Get - {userId}: No datastores for user", LogLevel.Information);
                return StatusCode(404, "No datastores for user");
            }
        }

        [HttpGet("{userId}/{dataStoreId}")]
        public IActionResult Get(string userId, string dataStoreId)
        {
            DataStore dataStore;
            try
            {
                dataStore = _dataStoreRepo.GetOne(userId, dataStoreId);
                _logger.Log("StorageController:Get - {userId}/{dataStoreId}: getting datastore " + dataStoreId, LogLevel.Trace);

                var json = JsonConvert.SerializeObject(dataStore);

                return Ok(JObject.Parse(json));
            }
            catch (KeyNotFoundException kyfe)
            {
                //What should I return? An Empty DataStore? Ideally a string but I can't do that.
                _logger.Log("StorageController:Get - {userId}/{dataStoreId}: A datastore with that id was not found for that user: " + kyfe, LogLevel.Error);
                return StatusCode(404, "no datastore with that id");
            }
        }

        // POST api/storage
        [HttpPost("{userId}")]
        public IActionResult Post([FromBody]JObject json, string userId)
        {
            try
            {
                var dataStore = DataStoreParser.Parse(json);
                dataStore.UserId = userId;

                //Creates a datastore for the user specified.
                var dataStoreId = _dataStoreRepo.Create(dataStore);
                _logger.Log("StorageController:Post {userId} :  datastore for user: " + dataStore.UserId + " created with an id of: " + dataStoreId, LogLevel.Trace);

                return Ok(JObject.FromObject(new { dataStoreId }));
            }
            catch (Exception except)
            {
                _logger.Log("StorageController:Post {userId} : Error in trying to create a datastore. Message: " + except.Message, LogLevel.Error);
                return StatusCode(500, except.Message);
            }
        }

        [HttpDelete("{userId}/{dataStoreId}")]
        public IActionResult DeleteOneDataStore(string userId, string dataStoreId)
        {
            try
            {
                var message = _dataStoreRepo.DeleteOneDataStore(userId, dataStoreId);
                _logger.Log("StorageController:Delete {userId}/{dataStoreId} - deleting datastore " + dataStoreId, LogLevel.Trace);
                var json = JsonConvert.SerializeObject(dataStoreId);
                return Ok(JObject.Parse(json));
            }
            catch (Exception except)
            {
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteAllDataStores(string userId)
        {
            try
            {
                var message = _dataStoreRepo.DeleteAllDataStores(userId);
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: deleting all datastores for user" + userId, LogLevel.Trace);
                var json = JsonConvert.SerializeObject(message);
                return Ok(JObject.Parse(json));
            }
            catch (Exception except)
            {
                _logger.Log("StorageController:Delete {userId}/{dataStoreId}: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }
    }
}