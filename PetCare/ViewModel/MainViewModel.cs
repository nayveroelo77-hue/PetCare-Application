using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        public partial bool IsPasswordHidden { get; set; } = true;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        public MainViewModel(DatabaseService databaseService, AuthService authService)
        {
            Title = "PetCare Login";
            _databaseService = databaseService;
            _authService = authService;
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Oops!", "Please enter both your email and password.", "OK");
                return;
            }

            var user = await _databaseService.GetUserAsync(Email, Password);
            if (user != null)
            {
                // Save Session
                _authService.SaveSession(user);

                await Shell.Current.DisplayAlertAsync("Success", $"Welcome back, {user.FullName}!", "Let's Go!");
                
                // Swap Shell based on role
                if (user.Role == "Admin")
                {
                    var adminViewModel = Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<AdminDashboardViewModel>();
                    if (Application.Current?.Windows.Count > 0 && adminViewModel != null)
                    {
                        Application.Current.Windows[0].Page = new Page.Admin.AdminShell(adminViewModel);
                    }
                }
                else
                {
                    var clientViewModel = Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ClientDashboardViewModel>();
                    if (Application.Current?.Windows.Count > 0 && clientViewModel != null)
                    {
                        Application.Current.Windows[0].Page = new Page.Client.ClientShell(clientViewModel);
                    }
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Login Failed", "Invalid email or password. Please try again.", "OK");
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
