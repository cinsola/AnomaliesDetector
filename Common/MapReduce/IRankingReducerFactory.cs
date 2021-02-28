using Common.Entities;
using Common.MapReduce.RankingReducers;

namespace Common.MapReduce
{
    public interface IRankingReducerFactory
    {
        IRankingReducer GetSensorReducer(ILogHeading header);
        float GetSensorEnvironmentContext(ILogHeading header, EnvironmentReference environment);
    }
}
