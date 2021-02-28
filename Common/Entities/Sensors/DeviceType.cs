using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceType
    {
        Thermometer,
        Humidity,
        Monoxide
    }
}
