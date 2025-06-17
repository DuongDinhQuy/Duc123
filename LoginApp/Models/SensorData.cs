namespace LoginApp.Models
{
    public class SensorData
    {
        public string UserId { get; set; } // Khóa ngoại

        public double? Temperature { get; set; }

        public double? Humidity { get; set; }

        public double? WaterLevel { get; set; }

        public DateTime? Time { get; set; }

            }
}