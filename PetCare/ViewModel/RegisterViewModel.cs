using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial string FullName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string MobileNumber { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsPasswordHidden { get; set; } = true;

        [ObservableProperty]
        public partial bool IsConfirmPasswordHidden { get; set; } = true;

        [ObservableProperty]
        public partial bool AgreeToTerms { get; set; } = false;

        public RegisterViewModel(DatabaseService databaseService)
        {
            Title = "Create Account";
            _databaseService = databaseService;
        }

        [RelayCommand]
        private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

        [RelayCommand]
        private void ToggleConfirmPasswordVisibility() => IsConfirmPasswordHidden = !IsConfirmPasswordHidden;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName) || FullName.Length < 2)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please enter a valid Full Name (min 2 characters).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please enter a valid Email.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Password must be at least 8 characters long.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Passwords do not match.", "OK");
                return;
            }

            if (!AgreeToTerms)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "You must agree to the Terms and Privacy Policy.", "OK");
                return;
            }

            var user = new UserAccount
            {
                FullName = FullName,
                Email = Email,
                MobileNumber = MobileNumber,
                Password = Password,
                Role = "Client",
                CreatedAt = DateTime.Now
            };

            var result = await _databaseService.SaveUserAsync(user);

            if (result == -1)
            {
                await Shell.Current.DisplayAlertAsync("Account Exists", "This email is already registered. Try logging in!", "OK");
            }
            else if (result > 0)
            {
                await Shell.Current.DisplayAlertAsync("Success", "Your PetCare account has been created successfully!", "Let's Login!");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Something went wrong. Please try again later.", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
