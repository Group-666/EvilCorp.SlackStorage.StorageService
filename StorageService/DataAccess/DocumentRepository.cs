using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using DomainTypes.Contracts;
using DomainTypes;

namespace DataAccess
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IMongoClient _client;
        private readonly ILogger _logger;
        private readonly String database = "StorageService";
        private readonly IMongoDatabase _db;

        public DocumentRepository(IMongoClient client)
        {
            _client = client;
            _logger = ConsoleFactory.CreateConsoleLogger();

            _db = _client.GetDatabase(database);
        }

        public string GetOne(string dataStoreId, string documentId)
        {
            //We want to get a document from a dataStore with a particular id.

            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            
            //Id is of type ObjectID, not string, so we need to parse it to an objectID first.            
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));          
            var document = collection.Find(filter).FirstOrDefault();
            _logger.Log("Trying to find a document with id: " + documentId, LogLevel.Trace);
                        
            var documentString = document.ToString();
            return document.ToString();
            
        }

        public string Insert(BsonDocument document, string dataStoreId)
        {
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            collection.InsertOne(document);
            var documentId = document["_id"].ToString();

            _logger.Log("Inserted document into " + dataStoreId, LogLevel.Information);
            _logger.Log("ID of inserted document " + documentId, LogLevel.Information);

            return documentId;
        }
        public string DeleteDocument(string userId, string dataStoreId, string documentId)
        {
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));
            collection.DeleteOne(filter);

            return "DeleteDocument";
        }

        public string DeleteData(string userId, string dataStoreId)
        {
            throw new NotImplementedException();
        }
    }
}
