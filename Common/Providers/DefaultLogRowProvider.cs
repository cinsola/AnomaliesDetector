using Common.Entities;
using Common.Exceptions;
using System;
using System.Globalization;

namespace Common.Providers
{
    public class DefaultLogRowProvider : ILogRowProvider
    {
        public LogFileRow Load(string line)
        {
            var tokens = line.Split(' ');
            try
            {
                if (DateTime.TryParse(tokens[0], out DateTime logDateTime) == true)
                {
                    return new LogFileRow(line, RowType.Log, logDateTime, float.Parse(tokens[1], CultureInfo.InvariantCulture), 
                        null, null);
                }
                else
                {
                    return new LogFileRow(line, RowType.Header, null, null, 
                        (DeviceType)Enum.Parse(typeof(DeviceType), tokens[0], true), tokens[1]);
                }
            }
            catch (Exception e)
            {
                throw new InvalidLogRowException($"Cannot read {line}. Invalid format", e);
            }

        }

        public EnvironmentReference LoadEnvironmentReference(string line)
        {
            try
            {
                var measures = line.Split(' ');
                return new EnvironmentReference(float.Parse(measures[1], CultureInfo.InvariantCulture), float.Parse(measures[2], CultureInfo.InvariantCulture), float.Parse(measures[3], CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                throw new InvalidLogException("Environment reference is missing or not valid", e);
            }
        }
    }
}
