using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.MapReduce.RankingReducers
{
    public class MeanBasedReducer : IRankingReducer
    {
        public readonly float UltraPreciseMeanEnvDiff;
        public readonly float UltraPreciseMaxSTandardDeviationDiff;
        public readonly float PreciseMaxStandardDeviationDiff;

        public MeanBasedReducer(float ultraPreciseMeanEnvDiff, float ultraPreciseMaxSTandardDeviationDiff, float preciseMaxStandardDeviationDiff)
        {
            UltraPreciseMeanEnvDiff = ultraPreciseMeanEnvDiff;
            UltraPreciseMaxSTandardDeviationDiff = ultraPreciseMaxSTandardDeviationDiff;
            PreciseMaxStandardDeviationDiff = preciseMaxStandardDeviationDiff;
        }

        public DevicePrecision Reduce(IEnumerable<ILogMeasure> log, float environmentContext)
        {
            var values = log.Select(x => x.Measure.GetValueOrDefault());
            var mean = values.Average();
            var stDev = Math.Sqrt(values.Average(v => Math.Pow(v - mean, 2)));

            if(Math.Abs(mean - environmentContext) <= UltraPreciseMeanEnvDiff)
            {
                if(stDev <= UltraPreciseMaxSTandardDeviationDiff)
                {
                    return DevicePrecision.UltraPrecise;
                }

                if (stDev <= PreciseMaxStandardDeviationDiff)
                {
                    return DevicePrecision.Precise;
                }
            }

            return DevicePrecision.Precise;
        }
    }
}
