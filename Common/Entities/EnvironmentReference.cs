using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common
{
    public class EnvironmentReference
    {
        public float Temperature { get; private set; }
        public float Humidity { get; private set; }
        public float Monoxide { get; private set; }

        public EnvironmentReference(float temperature, float humidity, float monoxide)
        {
            Temperature = temperature;
            Humidity = humidity;
            Monoxide = monoxide;
        }

        internal string Serialize()
        {
            return $"{Temperature.ToString(CultureInfo.InvariantCulture)}|{Humidity.ToString(CultureInfo.InvariantCulture)}|{Monoxide.ToString(CultureInfo.InvariantCulture)}";
        }
        internal static EnvironmentReference Deserialize(string serialized)
        {
            var tokens = serialized.Split('|');
            return new EnvironmentReference(float.Parse(tokens[0], CultureInfo.InvariantCulture), 
                float.Parse(tokens[1], CultureInfo.InvariantCulture), 
                float.Parse(tokens[2], CultureInfo.InvariantCulture));
        }
    }
}
