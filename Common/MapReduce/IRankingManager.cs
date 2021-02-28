using Common.Entities;
using System.IO;

namespace Common.MapReduce
{
    public interface IRankingManager
    {
        DeviceDetails GetRanking(SingleSensorReadingsLog log);
        SingleSensorReadingsLog FromSingleSensorReadings(Stream readings);
    }
}
