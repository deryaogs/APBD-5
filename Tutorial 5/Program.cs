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


