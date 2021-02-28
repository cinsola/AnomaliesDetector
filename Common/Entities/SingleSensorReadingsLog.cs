using System.Collections.Generic;

namespace Common.Entities
{
    public class SingleSensorReadingsLog
    {
        public EnvironmentReference Environment { get; set; }
        public ILogHeading DeviceInfo { get; set; }
        public IEnumerable<ILogMeasure> Values { get; set; }
    }
}