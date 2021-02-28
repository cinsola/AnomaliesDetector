using Common.MapReduce.RankingReducers;

namespace Common.Entities.Sensors
{
    public class MonoxideSensor : ISensor

    {
        public DeviceType AsDevice { get => DeviceType.Monoxide; }
        public IRankingReducer RankingReducer { get => new DiscardKeepReducer(3); }
        public float EnvironmentContext(EnvironmentReference reference)
        {
            return reference.Monoxide;
        }

    }
}
