using Common.Entities;
using Common.Providers;
using System.Collections.Generic;
using System.IO;

namespace Common.MapReduce
{

    public class BaseRankingManager : IRankingManager
    {
        IRankingReducerFactory _reducerFactory { get; set; }
        ILogRowProvider _logRowProvider { get; set; }
        public BaseRankingManager(IRankingReducerFactory reducerFactory, ILogRowProvider rowProvider)
        {
            _reducerFactory = reducerFactory;
            _logRowProvider = rowProvider;
        }

        public DeviceDetails GetRanking(SingleSensorReadingsLog logFile)
        {
            var contextProp = _reducerFactory.GetSensorEnvironmentContext(logFile.DeviceInfo, logFile.Environment);
            var reducer = _reducerFactory.GetSensorReducer(logFile.DeviceInfo);
            var ranking = reducer.Reduce(logFile.Values, contextProp);
            return new DeviceDetails(logFile.DeviceInfo.Name, ranking);
        }

        public SingleSensorReadingsLog FromSingleSensorReadings(Stream readings)
        {
            readings.Position = 0;
            var log = new List<ILogMeasure>();
            using (var reader = new StreamReader(readings))
            {
                var environmentContext = EnvironmentReference.Deserialize(reader.ReadLine());
                var heading = _logRowProvider.Load(reader.ReadLine());
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    log.Add(_logRowProvider.Load(line));
                }

                return new SingleSensorReadingsLog { DeviceInfo = heading, Values = log, Environment = environmentContext };
            }
        }
    }
}
