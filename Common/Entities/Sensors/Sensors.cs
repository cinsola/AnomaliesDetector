using Common.MapReduce.RankingReducers;

namespace Common.Entities
{
    public interface ISensor
    {
        DeviceType AsDevice { get; }
        IRankingReducer RankingReducer { get; }
        float EnvironmentContext(EnvironmentReference reference);
    }
}
