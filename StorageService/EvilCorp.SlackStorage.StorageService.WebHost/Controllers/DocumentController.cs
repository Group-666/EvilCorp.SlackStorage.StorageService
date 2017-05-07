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
            _logger = ConsoleFactory.CreateLogger();

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
                _logger.Log("DocumentController:Get - {userId}/{dataStoreId}/data/{documentId} : Document retrieved from datastore " + document, LogLevel.Trace);
                return document;
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController:Get - {userId}/{dataStoreId}/data/{documentId} : " + except.Message, LogLevel.Critical);
                return except.Message;
            }
          
        }
        [HttpGet("{userId}/{dataStoreId}/data")]
        public string GetAll(string userId, string dataStoreId)
        {

           
            //Json prettifier in the broswer isn't making the json very pretty.
            //Perhaps because the broswer interprets the data as strings rather than json.
            var documents = _documentRepo.GetAll(dataStoreId);
            _logger.Log("DocumentController:Get - { userId}/{ dataStoreId}/ data /{ documentId} : documents " + documents, LogLevel.Trace);
            return documents;

        }

        [HttpPost("{userId}/{dataStoreId}")]
        public String Post([FromBody]JObject json, string userId, string dataStoreId)
        {
            //Insert a document into a datastore.
            try
            {
                var doc = BsonDocument.Parse(json.ToString());
                var docId = _documentRepo.Insert(doc, dataStoreId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}: Doc to be inserted " + doc, LogLevel.Trace);
                return docId;
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}: Doc to be inserted " +except.Message, LogLevel.Critical);
                return except.Message;
            }

        }
        [HttpDelete("{userId}/{dataStoreId}/data/{documentId}")]
        public IActionResult DeleteOne(string userId, string dataStoreId, string documentId)
        {
            try
            {
                _documentRepo.DeleteDocument(userId, dataStoreId, documentId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/{documentId}:  Deleting document with id " + documentId, LogLevel.Trace);
                return Ok("Funky female baboons");
            }
            catch (Exception except)
            {
                _logger.Log("DocumentController: Post - { userId}/{ dataStoreId}/ data /{ documentId}: " + except.Message, LogLevel.Critical);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("{userId}/{dataStoreId}/data")]
        public string DeleteAllData(string userId, string dataStoreId)
        {
            try
            {
                _documentRepo.DeleteData(userId, dataStoreId);
                _logger.Log("DocumentController:Post - {userId}/{dataStoreId}/data/: deleting all data from datastore " + dataStoreId, LogLevel.Trace);
                return "data exterminated from dataStore: " + dataStoreId;
            }
            catch(Exception except)
            {
                _logger.Log("DocumentController: Post - { userId}/{ dataStoreId}/ data /: " +  except.Message, LogLevel.Critical);
                return except.Message;
            }
            
        }
    }
}