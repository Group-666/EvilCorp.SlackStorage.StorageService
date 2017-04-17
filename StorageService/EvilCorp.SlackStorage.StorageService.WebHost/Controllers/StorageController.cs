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
        //private readonly ILogger _logger;
        
        public StorageController()
        {
            //_logger = logger;  
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

                               
                StorageRepository dataStoreRepo = new StorageRepository(new MongoClient("mongodb://127.0.0.1:32768/"),new ConsoleLogger(LogLevel.Critical));

                
                //Creates a datastore for the given user.
                var dataStoreId = dataStoreRepo.Create(dataStore);

                return dataStoreId;
               
            }
            catch (Exception ex)
            {
                //Log Error
                //Also what do I return? Want to return the db ID if it's been created,
                //but what do I do if something goes wrong?
                return ex.Message;
            }
        }

 
    }
}
