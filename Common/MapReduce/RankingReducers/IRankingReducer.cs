using Common.Entities;
using System.Collections.Generic;

namespace Common.MapReduce.RankingReducers
{
    public interface IRankingReducer
    {
        DevicePrecision Reduce(IEnumerable<ILogMeasure> values, float environmentContext);
    }
}
