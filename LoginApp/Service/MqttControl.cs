using System;
using System.Threading.Tasks;
using MQTT;

namespace LoginApp.Service
{
    public class MQTTControlService
    {
        private readonly MQTTDeviceClient mqttClient;
        private readonly string controlTopic;

        // Sự kiện thông báo kết quả gửi lệnh (có thể mở rộng thêm phản hồi từ thiết bị)
        public event Action<string>? OnCommandSent;
        public event Action<string>? OnError;

        public bool IsConnected { get; private set; } = false;

        public MQTTControlService(
            string brokerAddress = "broker.emqx.io",
            int brokerPort = 8883,
            string controlTopic = "duc/relay",
            string clientId = "maui-control-client")
        {
            this.controlTopic = controlTopic;

            mqttClient = new MQTTDeviceClient(
                brokerAddress: brokerAddress,
                brokerPort: brokerPort,
                topic: controlTopic,
                clientId: clientId
            );
        }

        // Kết nối tới MQTT broker
        public async Task StartAsync(string? username = null, string? password = null)
        {
            try
            {
                await mqttClient.ConnectAsync(username, password);
                IsConnected = true;
                Console.WriteLine("[MQTTControlService] Đã kết nối tới MQTT broker.");
            }
            catch (Exception ex)
            {
                IsConnected = false;
                OnError?.Invoke($"Lỗi khi kết nối MQTT: {ex.Message}");
                throw;
            }
        }

        // Ngắt kết nối
        public async Task StopAsync()
        {
            try
            {
                if (IsConnected)
                {
                    await mqttClient.DisconnectAsync();
                    IsConnected = false;
                    Console.WriteLine("[MQTTControlService] Đã ngắt kết nối MQTT broker.");
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Lỗi khi ngắt kết nối MQTT: {ex.Message}");
            }
        }

        // Gửi lệnh bật/tắt relay (không phân biệt relay nào)
        public async Task SendRelayCommandAsync(bool state)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Chưa kết nối MQTT broker.");

            try
            {
                // Gửi lệnh "relay" với giá trị true (on)/false (off)
                await mqttClient.SendCommandAsync("relay", state);

                Console.WriteLine($"[MQTTControlService] Đã gửi lệnh relay: {(state ? "ON" : "OFF")}");
                OnCommandSent?.Invoke($"Relay: {(state ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Lỗi khi gửi lệnh MQTT: {ex.Message}");
            }
        }
    }
}