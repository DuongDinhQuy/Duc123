using System;
using System.Threading.Tasks;
using MQTT;

namespace LoginApp.Service
{
    public class MQTTService
    {
        private readonly MQTTDeviceClient client;

        // Sự kiện truyền dữ liệu cảm biến: nhiệt độ, độ ẩm
        public event Action<double, double>? OnSensorDataReceived;

        // Sự kiện lỗi (mở rộng nếu cần)
        public event Action<string>? OnError;

        // Trạng thái kết nối
        public bool IsConnected { get; private set; } = false;

        public MQTTService(
            string brokerAddress = "broker.emqx.io",
            int brokerPort = 8883,
            string topic = "hello/esp32",
            string clientId = "maui-client")
        {
            client = new MQTTDeviceClient(
                brokerAddress: brokerAddress,
                brokerPort: brokerPort,
                topic: topic,
                clientId: clientId
            );

            // Gắn sự kiện nhận dữ liệu từ client
            client.OnSensorDataReceived += (temp, hum) =>
            {
                OnSensorDataReceived?.Invoke(temp, hum);
            };
        }

        // Kết nối tới MQTT broker, có thể truyền username/password nếu cần
        public async Task StartAsync(string? username = null, string? password = null)
        {
            try
            {
                await client.ConnectAsync(username, password);
                IsConnected = true;
                Console.WriteLine("[MQTTService] Đã kết nối tới MQTT broker.");
            }
            catch (Exception ex)
            {
                IsConnected = false;
                OnError?.Invoke($"Lỗi khi kết nối MQTT: {ex.Message}");
                Console.WriteLine($"[MQTTService] Lỗi khi kết nối MQTT: {ex.Message}");
                throw;
            }
        }

        // Ngắt kết nối khỏi broker
        public async Task StopAsync()
        {
            try
            {
                if (IsConnected)
                {
                    await client.DisconnectAsync();
                    IsConnected = false;
                    Console.WriteLine("[MQTTService] Đã ngắt kết nối MQTT broker.");
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Lỗi khi ngắt kết nối MQTT: {ex.Message}");
                Console.WriteLine($"[MQTTService] Lỗi khi ngắt kết nối MQTT: {ex.Message}");
            }
        }

        // Gửi lệnh điều khiển thiết bị (VD: "pump", true/false)
        public async Task SendCommandAsync(string device, bool state)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Chưa kết nối MQTT broker.");
            }

            try
            {
                await client.SendCommandAsync(device, state);
                Console.WriteLine($"[MQTTService] Đã gửi lệnh: {device} = {state}");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Lỗi khi gửi lệnh MQTT: {ex.Message}");
                Console.WriteLine($"[MQTTService] Lỗi khi gửi lệnh MQTT: {ex.Message}");
            }
        }
    }
}
