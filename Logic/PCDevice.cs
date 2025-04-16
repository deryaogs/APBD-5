public class PCDevice : Device
{
    public PCDevice(string id, string name, bool isEnabled, string operatingSystem)
    {
        Id = id;
        Name = name;
        IsEnabled = isEnabled;
        OperatingSystem = operatingSystem;
    }
}