using Microsoft.AspNetCore.Mvc;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DeviceRepository>();
builder.Services.AddSingleton<DeviceManager>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

[Route("api/[controller]")]
[ApiController]
public class DevicesController : ControllerBase
{
    private readonly DeviceManager _deviceService;

    public DevicesController(DeviceManager deviceService)
    {
        _deviceService = deviceService;
    }

    // GET: api/Devices
    [HttpGet]
    public IActionResult GetAll()
    {
        var devices = _deviceService.GetAllDevices()
            .Select(d => new { d.Id, d.Name, d.IsEnabled, d.OperatingSystem });
        return Ok(devices);
    }

    // GET: api/Devices/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var device = _deviceService.GetDeviceById(id);
        return device is not null ? Ok(device) : NotFound();
    }

    // POST: api/Devices
    [HttpPost]
    public IActionResult Create([FromBody] Device device)
    {
        var created = _deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/Devices/{id}
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Device device)
    {
        device.Id = id;  // Ensure the device's Id stays consistent
        var updatedDevice = _deviceService.EditDevice(device);
        return updatedDevice is not null ? Ok(updatedDevice) : NotFound();
    }

    // DELETE: api/Devices/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var removedDevice = _deviceService.RemoveDeviceById(id);
        return removedDevice is not null ? Ok() : NotFound();
    }
}




public class Device
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string OperatingSystem { get; set; }
}


public class DeviceRepository
{
    private readonly List<Device> _devices;

    public DeviceRepository()
    {
        _devices = new List<Device>
        {
            new Device { Id = "P-1", Name = "PC 1", IsEnabled = true, OperatingSystem = "Windows 10" },
            new Device { Id = "P-2", Name = "PC 2", IsEnabled = true, OperatingSystem = "Ubuntu" }
        };
    }

    public IEnumerable<Device> GetAll() => _devices;

    public Device? GetById(string id) => _devices.FirstOrDefault(d => d.Id == id);

    public Device Add(Device device)
    {
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

public class DeviceManager
{
    private readonly DeviceRepository _deviceRepository;

    public DeviceManager(DeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public IEnumerable<Device> GetAllDevices() => _deviceRepository.GetAll();

    public Device? GetDeviceById(string id) => _deviceRepository.GetById(id);

    public Device AddDevice(Device newDevice) => _deviceRepository.Add(newDevice);

    public Device? EditDevice(Device updatedDevice) => _deviceRepository.Update(updatedDevice);

    public Device? RemoveDeviceById(string deviceId) => _deviceRepository.Remove(deviceId);
}