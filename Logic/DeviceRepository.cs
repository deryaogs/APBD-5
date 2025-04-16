static void Main()
{
    
}

public class DeviceRepository
{
    private readonly List<Device> _devices;

    public DeviceRepository()
    {
        _devices = new List<Device>
        {
            new PCDevice("P-1", "PC 1", true, "Windows 10"),
            new PCDevice("P-2", "PC 2", true, "Ubuntu")
        };
    }

    public IEnumerable<Device> GetAll() => _devices;

    public Device? GetById(string id) => _devices.FirstOrDefault(d => d.Id == id);

    public Device Add(Device device, int max)
    {
        if(_devices.Count() >= max) {
            throw new Exception();
        }
        if (_devices.Any(d => d.Id == device.Id))
        {
            throw new InvalidOperationException("Device already exists.");
        }

        _devices.Add(device);
        return device;
    }

    public Device? Update(Device device)
    {
        var existingDevice = _devices.FirstOrDefault(d => d.Id == device.Id);
        if (existingDevice != null)
        {
            existingDevice.Name = device.Name;
            existingDevice.IsEnabled = device.IsEnabled;
            existingDevice.OperatingSystem = device.OperatingSystem;
            return existingDevice;
        }
        return null;
    }

    public Device? Remove(string id)
    {
        var device = _devices.FirstOrDefault(d => d.Id == id);
        if (device != null)
        {
            _devices.Remove(device);
            return device;
        }
        return null;
    }
}
