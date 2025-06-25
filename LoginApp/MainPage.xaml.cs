using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LoginApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            FakeSensorData();
        }

        /// <summary>
        /// Hàm fake dữ liệu cảm biến và cập nhật UI liên tục mỗi 2 giây
        /// </summary>
        private async void FakeSensorData()
        {
            // Giá trị cố định
            double fakeTemp = 45.0; // Nhiệt độ cao bất thường
            double fakeHumidity = 78.0;
            double fakeWater = 100.0;

            while (true)
            {
                var args = new SensorDataEventArgs
                {
                    Temperature = fakeTemp,
                    Humidity = fakeHumidity,
                    WaterLevel = fakeWater
                };

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TemperatureLabel.Text = args.Temperature.HasValue ? args.Temperature.Value.ToString("F1") : "--";
                    HumidityLabel.Text = args.Humidity.HasValue ? args.Humidity.Value.ToString("F1") : "--";
                    WaterLevelLabel.Text = args.WaterLevel.HasValue ? args.WaterLevel.Value.ToString("F2") : "--";

                    // Nếu nhiệt độ quá cao, hiện cảnh báo
                    if (args.Temperature.HasValue && args.Temperature.Value >= 45)
                    {
                        // Hiện cảnh báo, chỉ hiện 1 lần mỗi chu kỳ
                        Application.Current?.MainPage?.DisplayAlert(
                            "CẢNH BÁO NHIỆT ĐỘ",
                            "Nhiệt độ quá cao, cần kiểm tra lại vườn!",
                            "Đã hiểu"
                        );
                    }
                });

                await PostSensorDataToServer(args);

                await Task.Delay(2000);
            }
        }

        // Đã loại bỏ hoàn toàn các thông báo lỗi
        private async Task PostSensorDataToServer(SensorDataEventArgs sensor)
        {
            try
            {
                var httpClient = new HttpClient();
                var sensorData = new
                {
                    UserId = Globals.GlobalUserId,
                    Temperature = sensor.Temperature,
                    Humidity = sensor.Humidity,
                    WaterLevel = sensor.WaterLevel,
                    Time = DateTime.UtcNow
                };

                await httpClient.PostAsJsonAsync("https://1ffc-2001-ee0-4161-2458-f59b-22cf-a5a0-7ee0.ngrok-free.app/api/SensorData", sensorData);
                // Không hiện thông báo nào cả dù lỗi hay thành công
            }
            catch
            {
                // Bỏ qua tất cả lỗi, không hiện bất kỳ thông báo nào
            }
        }
    }

    public class SensorDataEventArgs : EventArgs
    {
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? WaterLevel { get; set; }
    }
}