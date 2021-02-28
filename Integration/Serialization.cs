using Common;
using Common.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Integration
{
    [TestClass]
    public class Serialization
    {
        [TestMethod]
        [TestCategory("Json contracts")]
        public void SerializeDeserialize()
        {
            string[] rows = new string[]
            {
                "thermometer temp-1",
                "2007-04-05T22:00 72.4"
            };

            var provider = AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>();

            Queue<LogFileRow> logFile = new Queue<LogFileRow>();
            logFile.Enqueue(provider.Load(rows[0]));
            logFile.Enqueue(provider.Load(rows[1]));
            var prepared = new LogFile(new EnvironmentReference(1, 2, 3), logFile);

            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(prepared);
            LogFile deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<LogFile>(serialized);
            Assert.IsTrue(deserialized.EnvironmentReference.Temperature == 1);
            Assert.IsTrue(deserialized.Queue.Peek().Name == "temp-1");
        }
    }
}
