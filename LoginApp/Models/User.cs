namespace LoginApp.Models
{
    public class UserModel
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public object sensorDatas { get; set; }
        public object deviceActions { get; set; }
    }
}