using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public interface IDocumentRepository
    {
        string Insert(BsonDocument document, string dataStoreId);
        string GetOne(string userId,string documentId);
        string GetAll(string dataStoreId);
        string DeleteDocument(string userId, string dataStoreId, string documentId);

        string DeleteData(string userId, string dataStoreId);
    }
}
