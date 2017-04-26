using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DomainTypes;
using DataAccess;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using DomainTypes.Contracts;
using MongoDB.Bson.Serialization;
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
            _logger = ConsoleFactory.CreateConsoleLogger();

            //Want to prevent hardcoding of the mongoClient IP. 
            _dataStoreRepo = new StorageRepository(new MongoClient("mongodb://127.0.0.1:32768/"));
        }
        // GET: api/storage/<userId>
        [HttpGet("{userId}")]
        public List<DataStore> Get(String userId)
        {
            List<DataStore> dataStores = _dataStoreRepo.GetAll(userId);
            return dataStores;
        }
        [HttpGet("{userId}/{dataStoreId}")]
        public DataStore Get(String userId, String dataStoreId)
        {
            DataStore dataStore;
            try
            {
                dataStore = _dataStoreRepo.GetOne(userId, dataStoreId);
            }
            catch (KeyNotFoundException kyfe)
            {
                //What should I return? An Empty DataStore? Ideally a string but I can't do that.
                _logger.Log("A datastore with that id was not found for that user", LogLevel.Error);
                return null;
            }
            

            return dataStore;
        }

        // POST api/storage
        [HttpPost("{userId}")]
        public String Post([FromBody]JObject json, String userId)
        {
            try
            {
                var dataStore = DataStoreParser.Parse(json);
                dataStore.UserId = userId;

                //Creates a datastore for the user specified.
                var dataStoreId = _dataStoreRepo.Create(dataStore);
                _logger.Log("datastore for user: " + dataStore.UserId + " created with an id of: " + dataStoreId, LogLevel.Information);
                
                return dataStoreId;

            }
            catch (Exception ex)
            {
                _logger.Log("Error in trying to create a datastore. Message: " + ex.Message, LogLevel.Error);
                return ex.Message;
            }
        }
        [HttpPost("{userId}/{dataStoreId}")]
        public String Post([FromBody]JObject json, String userId, String dataStoreId)
        {
            //Don't really need to parse this do I...
            //var bson = json.ToBson();

            //_logger.Log(bsonDoc.ToString(), LogLevel.Information);
            //var document = new Document(bsonDoc);
            var doc = BsonDocument.Parse(json.ToString());
            _logger.Log(doc + "", LogLevel.Information);
            var docId = _dataStoreRepo.Insert(doc, dataStoreId);
            return docId;
        }
     


 
    }
}
