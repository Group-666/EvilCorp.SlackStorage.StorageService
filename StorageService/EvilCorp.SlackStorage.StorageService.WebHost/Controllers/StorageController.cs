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


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvilCorp.SlackStorage.StorageService.WebHost.Controllers
{
    
    [Route("api/[controller]")]
    public class StorageController : Controller
    {
        private readonly IConfiguration _config;
        public StorageController()
        {
            //_config = config;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

  

        // POST api/storage
        [HttpPost]
        public String Post([FromBody]JObject json)
        {
            try
            {
                var dataStore = DataStoreParser.Parse(json);

                //I'm hoping this will get me the database connection string.
                //something wrong with connection string.
                //var connectionString = _config["DatabaseConnectionString"];


                //10.0.0.10:32768
                StorageRepository dataStoreRepo = new StorageRepository(new MongoClient("mongodb://127.0.0.1:32768/"));

                
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
