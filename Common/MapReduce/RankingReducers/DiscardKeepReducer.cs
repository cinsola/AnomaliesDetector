using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.MapReduce.RankingReducers
{
    public class DiscardKeepReducer : IRankingReducer
    {
        private readonly float Threshold;
        public DiscardKeepReducer(float threshold)
        {
            Threshold = threshold;
        }
        public DevicePrecision Reduce(IEnumerable<ILogMeasure> log, float environmentContext)
        {
            if (log.Any(x => Math.Abs(x.Measure.Value - environmentContext) > Threshold)) return DevicePrecision.Discard;
            return DevicePrecision.Keep;
        }
    }
}
