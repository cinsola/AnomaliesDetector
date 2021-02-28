using Common.Entities;
using Common.Exceptions;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Common
{
    public class LogFileRow : ILogHeading, ILogMeasure
    {
        public string OriginalRow { get; private set; }
        public RowType Type { get; private set; }
        public DateTime? LogDateTime { get; private set; }
        public float? Measure { get; private set; }

        public DeviceType? DeviceType { get; private set; }
        public string Name { get; private set; }

        [JsonConstructor()]
        public LogFileRow(string originalRow, RowType type, DateTime? logDateTime, float? measure, DeviceType? deviceType, string name)
        {
            OriginalRow = originalRow;
            Type = type;
            LogDateTime = logDateTime;
            Measure = measure;
            DeviceType = deviceType;
            Name = name;
        }
    }
}
