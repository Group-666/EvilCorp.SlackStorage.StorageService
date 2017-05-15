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
using Newtonsoft.Json;

namespace EvilCorp.SlackStorage.StorageService.WebHost.Controllers
{
    [Route("api/Storage")]
    public class DocumentController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDocumentRepository _documentRepo;

        public DocumentController()
        {
            _logger = ConsoleFactory.CreateLogger();

            //Want to prevent hardcoding of the mongoClient IP.
            //
            //mongodb://localhost:27017/
            _documentRepo = new DocumentRepository(new MongoClient("mongodb://localhost:27017/"));
        }

        [HttpGet("{userId}/{dataStoreId}/data/{documentId}")]
        public IActionResult Get(String userId, String dataStoreId, String documentId)
        {
            try
            {
                //var json = JsonConvert.SerializeObject
                //Get a particular document from a datastore. Not actually using the userId for anything here.
                
                var document = _documentRepo.GetOne(dataStoreId, documentId);
                if (document.Length != 0)
                {
                    var json = JsonConvert.SerializeObject(document);
                    _logger.Log("DocumentController:Get - {userId}/{dataStoreId}/data/{documentId} : Document retrieved from datastore " + document, LogLevel.Trace);
                    return Ok(JObject.Parse(json));
                }
                else
                {
                    _logger.Log("DocumentController: Get - { userId}/{ dataStoreId}/ data /{ documentId} : Document " + documentId + " does not exist", LogLevel.Information);
                    return StatusCode(204, "That document does not exist");
                }
              
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController:Get - {userId}/{dataStoreId}/data/{documentId} : " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }

        [HttpGet("{userId}/{dataStoreId}/data")]
        public IActionResult GetAll(string userId, string dataStoreId)
        {
          
            var documents = _documentRepo.GetAll(dataStoreId);

            if (documents.Length != 0)
            {
                _logger.Log("DocumentController:Get - { userId}/{ dataStoreId}/ data /{ documentId} : documents " + documents, LogLevel.Trace);
                var json = JsonConvert.SerializeObject(documents);
                return Ok(JObject.Parse(json));
            }
            else
            {
                _logger.Log("DocumentController:Get - { userId}/{ dataStoreId}/ data /{ documentId} No documents for that user", LogLevel.Information);
                return StatusCode(204, "No documents for user");
            }

         
        }

        [HttpPost("{userId}/{dataStoreId}")]
        public IActionResult Post([FromBody]JObject json, string userId, string dataStoreId)
        {
            //Insert a document into a datastore.
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                var docId = _documentRepo.Insert(doc, dataStoreId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}: Doc to be inserted " + doc, LogLevel.Trace);

                var jsonString = JsonConvert.SerializeObject(docId); 
                return Ok(JObject.Parse(jsonString));
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}: Doc to be inserted " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }

        [HttpDelete("{userId}/{dataStoreId}/data/{documentId}")]
        public IActionResult DeleteOne(string userId, string dataStoreId, string documentId)
        {
            try
            {
                var docId = _documentRepo.DeleteDocument(userId, dataStoreId, documentId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/{documentId}:  Deleting document with id " + documentId, LogLevel.Trace);

                var json = JsonConvert.SerializeObject(docId);

                return Ok(JObject.Parse(json));
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController: Post - { userId}/{ dataStoreId}/ data /{ documentId}: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }

        [HttpDelete("{userId}/{dataStoreId}/data")]
        public IActionResult DeleteAllData(string userId, string dataStoreId)
        {
            try
            {
                _documentRepo.DeleteData(userId, dataStoreId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/: deleting all data from datastore " + dataStoreId, LogLevel.Trace);
                var json = JsonConvert.SerializeObject("data for " + userId + " removed");
                return Ok(JObject.Parse(json));
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController: Post - { userId}/{ dataStoreId}/ data /: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }

        [HttpPut("{userId}/{dataStoreId}/data/{documentId}")]
        public IActionResult updateDocument(string userId, string dataStoreId, string documentId, [FromBody]JObject json)
        {
            //Find particular document.
            //Replace said document with whatever is in the json.
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                _documentRepo.UpdateDocument(dataStoreId, documentId, doc);

                var jsonString = JsonConvert.SerializeObject(documentId);
                return Ok(JObject.Parse(jsonString));
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController: Put - {userId}/{ dataStoreId}/ data /{ documentId}: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }

        }
    }
}