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

    [HttpGet]
    public IResult GetAll()
    {
        var devices = _deviceService.GetAllDevices()
            .Select(d => new { d.Id, d.Name, d.IsEnabled, d.OperatingSystem });
        return Results.Ok(devices); 
    }

    [HttpGet("{id}")]
    public IResult GetById(string id)
    {
        var device = _deviceService.GetDeviceById(id);
        return device is not null ? Results.Ok(device) : Results.NotFound();
    }

    [HttpPost]
    public IResult Create([FromBody] Device device)
    {
        var created = _deviceService.AddDevice(device);
        return Results.Created($"/api/devices/{created.Id}", created); 
    }

    [HttpPut("{id}")]
    public IResult Update(string id, [FromBody] Device device)
    {
        device.Id = id; 
        var updatedDevice = _deviceService.EditDevice(device);
        return updatedDevice is not null ? Results.Ok(updatedDevice) : Results.NotFound();
    }

    [HttpDelete("{id}")]
    public IResult Delete(string id)
    {
        var removedDevice = _deviceService.RemoveDeviceById(id);
        return removedDevice is not null ? Results.Ok() : Results.NotFound();
    }
}




public abstract class Device
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string OperatingSystem { get; set; }
}

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