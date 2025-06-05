using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using LoginApp.Service;

namespace LoginApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MQTTService mqttService;
        private readonly MQTTControlService relayService;
        private bool relayOn = false;

        public MainPage()
        {
            InitializeComponent();

            mqttService = new MQTTService();
            mqttService.OnSensorDataReceived += OnSensorDataReceived;
            mqttService.OnError += OnMQTTError;

            // Khởi tạo service điều khiển relay
            relayService = new MQTTControlService();

            ConnectToMQTT();
            ConnectToRelay();
        }

        private async void ConnectToMQTT()
        {
            try
            {
                await mqttService.StartAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi kết nối", $"Không thể kết nối MQTT: {ex.Message}", "OK");
            }
        }

        // Kết nối relay service nếu cần thiết (thường dùng chung broker, có thể chỉ dùng 1 kết nối nếu broker/topic giống nhau)
        private async void ConnectToRelay()
        {
            try
            {
                await relayService.StartAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi kết nối", $"Không thể kết nối MQTT (relay): {ex.Message}", "OK");
            }
        }

        private void OnSensorDataReceived(double temp, double hum)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TemperatureLabel.Text = $"{temp:F1}";
                HumidityLabel.Text = $"{hum:F1}%";
            });
        }

        private void OnMQTTError(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Lỗi MQTT", message, "OK");
            });
        }

        // Xử lý nút điều khiển relay
        private async void RelayToggleButton_Click(object sender, EventArgs e)
        {
            relayOn = !relayOn;
            RelayToggleButton.Text = relayOn ? "Tắt Relay" : "Bật Relay";
            RelayToggleButton.BackgroundColor = relayOn ? Colors.OrangeRed : Colors.LightGreen;

            try
            {
                await relayService.SendRelayCommandAsync(relayOn);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi relay", ex.Message, "OK");
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (mqttService != null && mqttService.IsConnected)
            {
                await mqttService.StopAsync();
            }
            if (relayService != null && relayService.IsConnected)
            {
                await relayService.StopAsync();
            }
        }
    }
}