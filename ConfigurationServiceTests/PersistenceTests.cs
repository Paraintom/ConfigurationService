using System.Collections.Generic;
using System.Linq;
using ConfigurationService;
using ConfigurationService.Persistence;
using NUnit.Framework;

namespace ConfigurationServiceTests
{
    internal class PersistenceTests : BaseTest
    {
        [Test]
        public void FirstReadReturnsEmptyList()
        {
            IStatePersister toTest = new StatePersister();
            toTest.Clean();
            var result = toTest.Read();
            Assert.IsEmpty(result);
        }

        [Test]
        public void TestPersistThenRead()
        {
            IStatePersister toTest = new StatePersister();
            var configurations = new List<Configuration>();
            for (int i = 0; i < 10; i++)
            {
                configurations.Add(GetFakeConfig(i));
            }
            toTest.Persist(configurations);
            var result = toTest.Read();
            Assert.AreEqual(result.Count, configurations.Count);
            Assert.IsTrue(result.All(configurations.Contains));
        }

        private Configuration GetFakeConfig(int i)
        {
            return new Configuration() { Instance = "inst" + i, Key = "key" + i, Value = "value" + i };
        }
    }
}
