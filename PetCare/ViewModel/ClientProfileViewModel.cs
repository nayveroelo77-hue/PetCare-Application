using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class ClientProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial string FullName { get; set; }

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string MobileNumber { get; set; }

        [ObservableProperty]
        public partial bool IsEditing { get; set; }

        [ObservableProperty]
        public partial string EditButtonText { get; set; } = "Edit Profile";

        public ClientProfileViewModel(AuthService authService, DatabaseService databaseService)
        {
            _authService = authService;
            _databaseService = databaseService;
            Title = "My Profile";
            
            FullName = _authService.UserName;
            Email = _authService.UserEmail;
        }

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            try
            {
                var user = await _databaseService.GetUserAsync(Email, string.Empty);
                if (user == null)
                {
                    var users = await _databaseService.GetUsersAsync();
                    user = users.FirstOrDefault(u => u.Email == Email);
                }
                if (user != null)
                {
                    FullName = user.FullName;
                    Email = user.Email;
                    MobileNumber = user.MobileNumber;
                }
            }
            catch { }
        }

        [RelayCommand]
        private void ToggleEdit()
        {
            IsEditing = !IsEditing;
            EditButtonText = IsEditing ? "Cancel Editing" : "Edit Profile";
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await Shell.Current.DisplayAlert("Validation", "Name cannot be empty.", "OK");
                return;
            }

            try
            {
                var users = await _databaseService.GetUsersAsync();
                var user = users.FirstOrDefault(u => u.Email == _authService.UserEmail);
                if (user == null) return;

                user.FullName = FullName;
                user.MobileNumber = MobileNumber;
                await _databaseService.UpdateUserAsync(user);

                // Update session
                _authService.SaveSession(user);

                IsEditing = false;
                EditButtonText = "Edit Profile";
                await Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to save profile.", "OK");
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _authService.ClearSession();
            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new UnauthenticatedShell();
            }
        }
    }
}
