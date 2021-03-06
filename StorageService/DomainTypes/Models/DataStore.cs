﻿using System;

namespace DomainTypes
{
    
    public class DataStore
    {
        public String DataStoreId{ get; set; }
        public String DataStoreName { get; set; }
        public String UserId{ get; set; }
        public DateTime CreatedAt { get; set; }

        public DataStore(String dataStoreId,String dataStoreName, String userId)
        {
            
            if (string.IsNullOrEmpty(dataStoreId))
                throw new ArgumentException("The dataStoreId cannot be null or empty.", nameof(dataStoreId));
            if (string.IsNullOrEmpty(dataStoreName))
                throw new ArgumentException("The dataStoreName cannot be null or empty.", nameof(dataStoreName));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("The userId cannot be null or empty.", nameof(userId));

            DataStoreId = dataStoreId;
            DataStoreName = dataStoreName;
            UserId = userId;
        }

        public DataStore(){ }

        public DataStore(string dataStoreName)
        {

            if (string.IsNullOrEmpty(dataStoreName))
                throw new ArgumentException("The dataStoreName cannot be null or empty.", nameof(dataStoreName));
        
            DataStoreName = dataStoreName;
            CreatedAt = DateTime.Now;
        }
       

        override
        public String ToString()
        {
            return "datastore to string";
        }
    }
}
