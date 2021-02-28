using Common.MapReduce.RankingReducers;

namespace Common.Entities.Sensors
{
    public class HumiditySensor : ISensor

    {
        public DeviceType AsDevice { get => DeviceType.Humidity; }
        public IRankingReducer RankingReducer { get => new DiscardKeepReducer(1); }

        public float EnvironmentContext(EnvironmentReference reference)
        {
            return reference.Humidity;
        }
    }
}
