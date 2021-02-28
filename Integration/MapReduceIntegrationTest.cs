using Common;
using Common.MapReduce;
using Common.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace Integration
{
    [TestClass]
    public class MapReduceIntegrationTest
    {
        [TestMethod]
        [TestCategory("File loading")]
        public async Task FileCanBeLoaded()
        {
            using (FileStream file = File.OpenRead("testData/dummyLog.txt"))
            {
                // prepare
                var _logRowProvider = AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>();
                var _rankingManager = AnomaliesDetectorContainer.Instance.Resolve<IRankingManager>();
                var mockObject = new Mock<SynchronizedMapReduce>(_logRowProvider, _rankingManager);
                mockObject.Setup(x => x.ProcessCurrentDelayed(It.IsAny<List<Task<DeviceDetails>>>(),
                    It.IsAny<EnvironmentReference>(),
                    It.IsAny<LogFile>())).CallBase();

                // act
                await mockObject.Object.LoadAndTriggerProcessFileAsync(file);

                // assert
                mockObject.Verify(c => c.ProcessBatch(It.IsAny<LogFile>()), Times.Exactly(6));

            }
        }

        [TestMethod]
        [TestCategory("File loading")]
        public async Task EnsureLogFileConsistency()
        {
            using (FileStream file = File.OpenRead("testData/dummyLogShort.txt"))
            {
                // prepare
                var _logRowProvider = AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>();
                var _rankingManager = AnomaliesDetectorContainer.Instance.Resolve<IRankingManager>();
                var mockObject = new Mock<SynchronizedMapReduce>(_logRowProvider, _rankingManager);

                LogFile tesLogFile = null;
                mockObject.Setup(x => x.ProcessBatch(It.IsAny<LogFile>()))
                    .Callback<LogFile>((input) =>
                    {
                        tesLogFile = input;
                    });
                mockObject.Setup(x => x.ProcessCurrentDelayed(It.IsAny<List<Task<DeviceDetails>>>(),
                    It.IsAny<EnvironmentReference>(), 
                    It.IsAny<LogFile>())).CallBase();

                //act
                await mockObject.Object.LoadAndTriggerProcessFileAsync(file);
                
                // assert
                mockObject.Verify(c => c.ProcessBatch(It.IsAny<LogFile>()), Times.Once);
                Assert.IsTrue(tesLogFile.Queue.Count == 3);
            }
        }


    }
}
