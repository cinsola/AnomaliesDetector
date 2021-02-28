using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities
{
    public interface ILogHeading
    {
        string Name { get; }
        DeviceType? DeviceType { get; }
    }
}
