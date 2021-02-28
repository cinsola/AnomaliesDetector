using Common.MapReduce.RankingReducers;

namespace Common.Entities.Sensors
{
    public class ThermometerSensor : ISensor

    {
        public DeviceType AsDevice { get => DeviceType.Thermometer; }
        public IRankingReducer RankingReducer { get => new MeanBasedReducer(0.5f, 3, 5); }
        public float EnvironmentContext(EnvironmentReference reference)
        {
            return reference.Temperature;
        }

    }
}
