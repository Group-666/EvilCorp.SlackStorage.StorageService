using System;
using System.Collections.Generic;
using System.Text;

namespace DomainTypes.Models
{
    public class MetaData
    {
        public String DataStoreId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Size { get; set; }
        public int NumDocuments { get; set; }


        public MetaData(String dataStoreId, DateTime createdAt)
        {
            DataStoreId = dataStoreId;
            CreatedAt = createdAt;
            
        }
    }
}
