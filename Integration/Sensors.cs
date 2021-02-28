using Common;
using Common.MapReduce;
using Common.MapReduce.RankingReducers;
using Common.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration
{
    [TestClass]
    public class Sensors
    {
        [TestMethod]
        [TestCategory("Sensor reducers")]
        public void TestSensor()
        {
            //prepare
            var provider = AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>();
            LogFileRow header = provider.Load("thermometer temp-1");

            //act
            var factory = AnomaliesDetectorContainer.Instance.Resolve<IRankingReducerFactory>();
            IRankingReducer reducer = factory.GetSensorReducer(header);


            Assert.IsTrue(reducer is MeanBasedReducer);
        }

        [TestMethod]
        [TestCategory("Sensor reducers")]
        public void TestSensorManagerImport()
        {
            using (FileStream file = File.OpenRead("testData/singleDeviceLog_blob.txt"))
            {
                // preapare
                var rankingManager = AnomaliesDetectorContainer.Instance.Resolve<IRankingManager>();

                //act
                var readingsLog = rankingManager.FromSingleSensorReadings(file);

                //assert
                Assert.IsTrue(readingsLog.Environment.Temperature == 70.0f);
                Assert.IsTrue(readingsLog.DeviceInfo.DeviceType == Common.Entities.DeviceType.Thermometer);
                Assert.IsTrue(readingsLog.Values.Count() == 2);
            }
        }

        [TestMethod]
        [TestCategory("Sensor reducers")]
        public async Task TestIntermediateFileMatch()
        {
            var expected = File.ReadAllText("testData/singleDeviceLog_blob.txt");
            using (FileStream file = File.OpenRead("testData/dummyLogShort.txt"))
            {
                // preapre
                var mockObject = new Mock<ETLBaseMapReduce>(AnomaliesDetectorContainer.Instance.Resolve<ILogRowProvider>());
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
                var stream = tesLogFile.ToSingleLogReadingsStream();

                // assert
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var actual = reader.ReadToEnd();
                    Assert.IsTrue(expected == actual);

                }
            }
        }

        [TestMethod]
        [TestCategory("Sensor reducers")]
        public async Task FullSyncronizedTest()
        {
            using (FileStream file = File.OpenRead("testData/dummyLog.txt"))
            {
                // prepare
                var etl = AnomaliesDetectorContainer.Instance.Resolve<ETLBaseMapReduce>();

                //act
                var result = (await etl.LoadAndTriggerProcessFileAsync(file)).ToList();

                //assert
                Assert.IsTrue(result[0].Precision == DevicePrecision.Precise && result[0].Name == "temp-1");
                Assert.IsTrue(result[1].Precision == DevicePrecision.UltraPrecise && result[1].Name == "temp-2");
                Assert.IsTrue(result[2].Precision == DevicePrecision.Keep && result[2].Name == "hum-1");
                Assert.IsTrue(result[3].Precision == DevicePrecision.Discard && result[3].Name == "hum-2");
                Assert.IsTrue(result[4].Precision == DevicePrecision.Keep && result[4].Name == "mon-1");
                Assert.IsTrue(result[5].Precision == DevicePrecision.Discard && result[5].Name == "mon-2");
            }
        }
    }
}
