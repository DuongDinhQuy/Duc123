using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LoginApp
{
    public partial class LoginPage : ContentPage
    {
        public static string GlobalUserId { get; private set; } // Biến toàn cục cho UserId

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "OK");
                return;
            }

            // Không gọi API, chỉ check tài khoản admin/1234
            if (username == "admin" && password == "1234")
            {
            
                await DisplayAlert("Thành công", "Đăng nhập thành công!", "OK");
                await Navigation.PushAsync(new MenuPage());
            }
            else
            {
                await DisplayAlert("Lỗi", "Sai tên đăng nhập hoặc mật khẩu.", "OK");
            }
        }

        private async void OnRegisterRedirectTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}