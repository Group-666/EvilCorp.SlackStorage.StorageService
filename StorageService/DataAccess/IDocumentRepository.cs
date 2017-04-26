using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    interface IDocumentRepository
    {
        String Insert(BsonDocument document, String dataStoreId);
        String GetOne(String documentId);
        String DeleteOne(String userId, String dataStoreId, String documentId);
        String DeleteData(String userId);
    }
}
