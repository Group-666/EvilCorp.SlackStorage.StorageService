using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using DataAccess;
using DomainTypes.Contracts;

namespace EvilCorp.SlackStorage.StorageService.WebHost.Controllers
{
    
    [Route("api/Storage")]
    public class DocumentController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDocumentRepository _documentRepo;

        public DocumentController()
        {
            _logger = ConsoleFactory.CreateConsoleLogger();

            //Want to prevent hardcoding of the mongoClient IP. 
            _documentRepo = new DocumentRepository(new MongoClient("mongodb://127.0.0.1:32768/"));
        }
        [HttpGet("{userId}/{dataStoreId}/data/{documentId}")]
        public String Get(String userId,String dataStoreId, String documentId)
        {
            try
            {
                //Get a particular document from a datastore. Not actually using the userId for anything here.
                var document = _documentRepo.GetOne(dataStoreId, documentId);
                return document;
            }
            catch (Exception except)
            {
                _logger.Log(except.Message, LogLevel.Error);
                return except.Message;
            }
          
        }

        [HttpPost("{userId}/{dataStoreId}")]
        public String Post([FromBody]JObject json, String userId, String dataStoreId)
        {
            //Insert a document into a datastore.
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                var docId = _documentRepo.Insert(doc, dataStoreId);
                return docId;
            }
            catch (Exception except)
            {
                _logger.Log(except.Message, LogLevel.Error);
                return except.Message;
            }

        }
    }
}