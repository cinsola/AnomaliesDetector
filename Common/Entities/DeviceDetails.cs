namespace Common
{
    public class DeviceDetails
    {
        public DevicePrecision Precision { get; set; }
        public string Name { get; set; }
        public DeviceDetails(string name, DevicePrecision precision)
        {
            Name = name;
            Precision = precision;
        }
        public DeviceDetails() { }
    }
}
