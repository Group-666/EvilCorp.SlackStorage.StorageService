using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using DomainTypes;
using DataAccess;

namespace EvilCorp.SlackStorage.Storage.UnitTests
{
    [TestClass]
    public class StorageRepositoryTests
    {
        [TestMethod,TestCategory("DataAccess")]
        public void StorageRepository_Can_Create_DataStore()
        {
            var dataStore = new DataStore("Samuel L jackson", "sam238as");
            var clientMock = CreateMongoMock(dataStore);

            StorageRepository storageRepository = new StorageRepository(clientMock);

            storageRepository.Create(dataStore);
        }

        private IMongoClient CreateMongoMock(DataStore dataStore)
        {
            var clientMock = new Mock<IMongoClient>();
            var dbMock = new Mock<IMongoDatabase>();
            var collectionMock = new Mock<IMongoCollection<DataStore>>();

            clientMock.Setup(c => c.GetDatabase(It.Is<string>(s => s == "StorageService"), null)).Returns(dbMock.Object);
            dbMock.Setup(d => d.GetCollection<DataStore>(It.Is<string>(s => s == "Storage"), null)).Returns(collectionMock.Object);
            

            return clientMock.Object;
        }
    }
}
