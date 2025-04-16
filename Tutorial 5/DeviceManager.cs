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