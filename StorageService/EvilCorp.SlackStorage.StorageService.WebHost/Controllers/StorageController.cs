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


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvilCorp.SlackStorage.StorageService.WebHost.Controllers
{
    
    [Route("api/[controller]")]
    public class StorageController : Controller
    {
        private readonly ILogger _logger;
        
        public StorageController()
        {
            _logger = ConsoleLoggerFactory.CreateConsoleLogger();
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "Storage One", "Storage Two" };
        }

        // POST api/storage
        [HttpPost]
        public String Post([FromBody]JObject json)
        {
            try
            {
                var dataStore = DataStoreParser.Parse(json);

                StorageRepository dataStoreRepo = new StorageRepository(new MongoClient("mongodb://127.0.0.1:32768/"));

                //Creates a datastore for the given user.
                var dataStoreId = dataStoreRepo.Create(dataStore);
                _logger.Log("datastore for user: " + dataStore.UserId + " created with an id of: " + dataStoreId, LogLevel.Information);

                return dataStoreId;

            }
            catch (Exception ex)
            {
                _logger.Log("Error in trying to create a datastore. Message: " + ex.Message, LogLevel.Error);
                return ex.Message;
            }
        }

 
    }
}
