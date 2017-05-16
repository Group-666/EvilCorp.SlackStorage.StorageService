using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using DomainTypes.Contracts;
using DomainTypes;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.IO;

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
            _logger = ConsoleFactory.CreateLogger();

            _db = _client.GetDatabase(database);
        }

        public string GetOne(string dataStoreId, string documentId)
        {
            //We want to get a document from a dataStore with a particular id.
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            
            //Id is of type ObjectID, not string, so we need to parse it to an objectID first.            
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));
            _logger.Log("DocumentRepository-GetOne: Filter to find documents: " + filter, LogLevel.Trace);
            var document = collection.Find(filter).FirstOrDefault();
            _logger.Log("DocumentRepository-GetOne: Trying to find a document with id " + documentId, LogLevel.Trace);
                        
            var documentString = document.ToString();
            return document.ToString();
            
        }
        public string GetAll(string dataStoreId)
        {
            //Something is mucking up when converting to a list. 
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);

            //Empty filter matches everything, so everything will be returned.
            var filter = Builders<BsonDocument>.Filter.Empty;
            _logger.Log("DocumentRepository-GetAll: Filter to find documentsd: " + filter, LogLevel.Trace);
            var documents = collection.Find(filter).ToList();
            _logger.Log("DocumentRepository-GetAll:Grabbing all documents for datastore with id " + dataStoreId, LogLevel.Trace);

            //Create list, convert each ele
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }; // key part
            string jsonDocs = documents.ToJson(jsonWriterSettings);



            return jsonDocs;
        }

        public string Insert(BsonDocument document, string dataStoreId)
        {
            
            var collections = _db.ListCollections();
            while (collections.MoveNext())
            {
                foreach(var datastore in collections.Current)
                {
                    if (datastore["name"].AsString.Equals(dataStoreId))
                    {
                        var collection = _db.GetCollection<BsonDocument>(dataStoreId);
                        collection.InsertOne(document);

                        var documentId = document["_id"].ToString();

                        _logger.Log("DocumentRepository-Insert: Inserted document into " + dataStoreId, LogLevel.Trace);
                        _logger.Log("DocumentRepository-Insert: ID of inserted document " + documentId, LogLevel.Trace);

                        return documentId;
                    }

                }
            }

           

            return "No datastore with that Id found";
        }
        public string DeleteDocument(string userId, string dataStoreId, string documentId)
        {
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));
            _logger.Log("DocumentRepository-DeleteDocument: Filter to find document to be deleted: " + filter , LogLevel.Trace);
            collection.DeleteOne(filter);
            _logger.Log("DocumentRepository-DeleteDocument: Document with id " + documentId + " in datastore " + dataStoreId + " for user " + userId + " deleted", LogLevel.Trace);

            return "DeleteDocument";
        }

        public string DeleteData(string userId, string dataStoreId)
        {
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);

            var filter = Builders<BsonDocument>.Filter.Empty;
            _logger.Log("DocumentRepository-DeleteData: Filter to find document to be deleted: " + filter, LogLevel.Trace);
            collection.DeleteMany(filter);
            _logger.Log("DocumentRepository-DeleteDocument: removing all documents from datastore " + dataStoreId, LogLevel.Trace);

            return "all documents gone";
        }

        public string UpdateDocument(string dataStoreId, string documentId, BsonDocument document)
        {
            var collection = _db.GetCollection<BsonDocument>(dataStoreId);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId));
            
            collection.ReplaceOne(filter, document);
            _logger.Log("DocumentRepository-UpdateDocument: Id of doc " + documentId, LogLevel.Information);
            return ""+documentId;
        }
    }
}
