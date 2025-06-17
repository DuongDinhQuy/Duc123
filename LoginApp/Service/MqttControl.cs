using System;
using System.Threading.Tasks;
using MQTT;

namespace LoginApp.Service
{
    public class MQTTControl
    {
        private readonly MQTTDeviceClient mqttClient;
        private readonly string controlTopic;

        public event Action<string>? OnCommandSent;
        public event Action<string>? OnError;

        public bool IsConnected { get; private set; } = false;

        public MQTTControlService(
            string userId,
            string brokerAddress = "broker.emqx.io",
            int brokerPort = 8883,
            string clientId = "maui-control-client")
        {
            // Kênh điều khiển: "UserId/Device"
            controlTopic = $"{userId}/Device";

            mqttClient = new MQTTDeviceClient(
                brokerAddress: brokerAddress,
                brokerPort: brokerPort,
                topic: controlTopic,
                clientId: clientId
            );
        }

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

        // Gửi lệnh đến thiết bị (pump, light, ...)
        public async Task SendDeviceCommandAsync(string cmd, bool state)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Chưa kết nối MQTT broker.");

            try
            {
                // value: 1 (on), 0 (off)
                await mqttClient.SendCommandAsync(cmd, state ? 1 : 0);

                Console.WriteLine($"[MQTTControlService] Đã gửi lệnh {cmd}: {(state ? "ON" : "OFF")}");
                OnCommandSent?.Invoke($"{cmd}: {(state ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Lỗi khi gửi lệnh MQTT: {ex.Message}");
            }
        }
    }
}
