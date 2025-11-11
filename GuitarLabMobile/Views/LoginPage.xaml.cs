using Entity.Dtos;
using GuitarLabMobile.Services;
using Microsoft.Maui.Controls;

namespace GuitarLabMobile.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _apiService;

        public LoginPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorLabel.Text = "Please enter email and password";
                ErrorLabel.IsVisible = true;
                return;
            }

            var loginRequest = new LoginRequestDto { Email = email, Password = password };
            var success = await _apiService.LoginAsync(loginRequest);

            if (success)
            {
                // Navigate to LessonsPage
                await Shell.Current.GoToAsync("//LessonsPage");
            }
            else
            {
                ErrorLabel.Text = "Invalid credentials";
                ErrorLabel.IsVisible = true;
            }
        }
    }
}