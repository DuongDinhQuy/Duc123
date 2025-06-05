using System;
using Microsoft.Maui.Controls;

namespace LoginApp
{
    public partial class LoginPage 
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            // ⚠️ TODO: Thay thế đoạn này bằng kiểm tra với cơ sở dữ liệu thật
            if (username == "admin" && password == "1234")
            {
                await DisplayAlert("Thành công", "Đăng nhập thành công!", "OK");

                // 👉 Điều hướng sang trang HomePage (bạn cần đã đăng ký trong AppShell)
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                await DisplayAlert("Lỗi", "Sai tên đăng nhập hoặc mật khẩu.", "OK");
            }
        }

        private async void OnRegisterRedirectTapped(object sender, EventArgs e)
        {
            // 👉 Điều hướng sang trang đăng ký
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
