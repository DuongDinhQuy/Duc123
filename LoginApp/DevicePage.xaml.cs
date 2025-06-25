using System;
using Microsoft.Maui.Controls;
using LoginApp.Service;

namespace LoginApp
{
    public partial class DevicePage : ContentPage
    {
        // Sử dụng Globals.GlobalUserId truyền vào đúng constructor
        private readonly MQTTControlService mqttService;

        // Trạng thái thiết bị
        private bool pumpOn = false;
        private bool lightOn = false;
        private bool valveOn = false;

        public DevicePage()
        {
            InitializeComponent();
            mqttService = new MQTTControlService(userId: Globals.GlobalUserId);
            SetButtonsEnabled(false);
            UpdateButtonText();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await mqttService.StartAsync();
                SetButtonsEnabled(true);
            }
            catch (Exception ex)
            {
                SetButtonsEnabled(false);
                await DisplayAlert("MQTT lỗi", ex.Message, "OK");
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await mqttService.StopAsync();
            SetButtonsEnabled(false);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            PumpButton.IsEnabled = enabled;
            LightButton.IsEnabled = enabled;
            ValveButton.IsEnabled = enabled;
        }

        private void UpdateButtonText()
        {
            PumpButton.Text = pumpOn ? "Tắt Bơm" : "Bật Bơm";
            LightButton.Text = lightOn ? "Tắt Đèn" : "Bật Đèn";
            ValveButton.Text = valveOn ? "Tắt Van Nước" : "Bật Van Nước";
        }

        private async void OnPumpButtonClicked(object sender, EventArgs e)
        {
            if (!mqttService.IsConnected)
            {
                await DisplayAlert("MQTT", "Chưa kết nối tới MQTT broker!", "OK");
                return;
            }
            pumpOn = !pumpOn;
            await mqttService.SendDeviceCommandAsync("pump", true);
            UpdateButtonText();
        }

        private async void OnLightButtonClicked(object sender, EventArgs e)
        {
            if (!mqttService.IsConnected)
            {
                await DisplayAlert("MQTT", "Chưa kết nối tới MQTT broker!", "OK");
                return;
            }
            lightOn = !lightOn;
            await mqttService.SendDeviceCommandAsync("light", true);
            UpdateButtonText();
        }

        private async void OnValveButtonClicked(object sender, EventArgs e)
        {
            if (!mqttService.IsConnected)
            {
                await DisplayAlert("MQTT", "Chưa kết nối tới MQTT broker!", "OK");
                return;
            }
            valveOn = !valveOn;
            await mqttService.SendDeviceCommandAsync("valve", true);
            UpdateButtonText();
        }
    }
}