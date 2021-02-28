using Common.Entities;
using Common.MapReduce.RankingReducers;
using System.Collections.Generic;

namespace Common.MapReduce
{
    public class RankingReducerFactory : IRankingReducerFactory
    {
        Dictionary<DeviceType, ISensor> RegisteredComponents;
        public RankingReducerFactory(Dictionary<DeviceType, ISensor> registeredComponents)
        {
            RegisteredComponents = registeredComponents;
        }
        public IRankingReducer GetSensorReducer(ILogHeading header)
        {
            return RegisteredComponents[header.DeviceType.Value].RankingReducer;
        }

        public float GetSensorEnvironmentContext(ILogHeading header, EnvironmentReference environment)
        {
            return RegisteredComponents[header.DeviceType.Value].EnvironmentContext(environment);
        }
    }
}
