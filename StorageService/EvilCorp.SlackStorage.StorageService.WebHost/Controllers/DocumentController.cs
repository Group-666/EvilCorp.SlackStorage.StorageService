using DataAccess;
using DomainTypes.Contracts;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;

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
            //mongodb://localhost:27017/
            _documentRepo = new DocumentRepository(new MongoClient("mongodb://localhost:27017"));
        }

        [HttpGet("{userId}/{dataStoreId}/data/{documentId}")]
        public IActionResult Get(string userId, string dataStoreId, string documentId)
        {
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }
            try
            {
                //var json = JsonConvert.SerializeObject
                //Get a particular document from a datastore. Not actually using the userId for anything here.

                var result = _documentRepo.GetOne(dataStoreId, documentId);
                if (result.Length != 0)
                {
                    //var json = JsonConvert.SerializeObject(document);
                    _logger.Log("DocumentController:Get - {userId}/{dataStoreId}/data/{documentId} : Document retrieved from datastore " + result, LogLevel.Trace);

                    return Ok(JObject.FromObject(new { result }));
                }
                else
                {
                    _logger.Log("DocumentController: Get - { userId}/{ dataStoreId}/ data /{ documentId} : Document " + documentId + " does not exist", LogLevel.Information);
                    return StatusCode(404, "That document does not exist");
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
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }

            var documents = _documentRepo.GetAll(dataStoreId);

            //Currently documents can contain an empty array [] which is why we must have more than 2 characters.
            if (documents.Length > 2)
            {
                _logger.Log("DocumentController:Get - { userId}/{ dataStoreId}/ data /{ documentId} : documents " + documents, LogLevel.Trace);
                return Ok(documents);
            }
            else
            {
                _logger.Log("DocumentController:Get - { userId}/{ dataStoreId}/ data /{ documentId} No documents for that user", LogLevel.Information);
                return StatusCode(404, "No documents for user");
            }
        }

        [HttpPost("{userId}/{dataStoreId}")]
        public IActionResult Post([FromBody]JObject json, string userId, string dataStoreId)
        {
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }
            //Insert a document into a datastore.
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                var elementId = _documentRepo.Insert(doc, dataStoreId);

                if (elementId.Equals("No datastore with that Id found"))
                {
                    return StatusCode(404, "No datastore with that Id found");
                }

                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}: Doc to be inserted " + doc, LogLevel.Trace);

                return Ok(JObject.FromObject(new { elementId }));
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
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }
            try
            {
                var elementId = _documentRepo.DeleteDocument(userId, dataStoreId, documentId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/{documentId}:  Deleting document with id " + documentId, LogLevel.Trace);

                return Ok(JObject.FromObject(new { elementId }));
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
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }
            try
            {
                _documentRepo.DeleteData(userId, dataStoreId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/: deleting all data from datastore " + dataStoreId, LogLevel.Trace);
                var result = "data for " + userId + " removed";
                return Ok(JObject.FromObject(new { result }));
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
            string[] userIdFromDataStore = dataStoreId.Split('_');
            if (!userIdFromDataStore[0].Equals(userId))
            {
                return StatusCode(404, "userId does not match userid in datastoreId. Either the userId or datastoreId is incorrect");
            }
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                _documentRepo.UpdateDocument(dataStoreId, documentId, doc);
                var elementId = documentId;
                //var jsonString = JsonConvert.SerializeObject(documentId);
                return Ok(JObject.FromObject(new { elementId }));
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController: Put - {userId}/{ dataStoreId}/ data /{ documentId}: " + except.Message, LogLevel.Critical);
                return StatusCode(500, except.Message);
            }
        }
    }
}