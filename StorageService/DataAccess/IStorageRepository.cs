using DomainTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public interface IStorageRepository
    {
        String Create(DataStore dataStore);
        List<DataStore> GetAll(string userId);
        DataStore GetOne(string userId, string dataStoreId);
    }
}
