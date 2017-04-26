using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using DomainTypes.Contracts;

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
        public string DeleteData(string userId)
        {
            throw new NotImplementedException();
        }

        public string DeleteOne(string userId, string dataStoreId, string documentId)
        {
            throw new NotImplementedException();
        }

        public string GetOne(string documentId)
        {
            throw new NotImplementedException();
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
    }
}
