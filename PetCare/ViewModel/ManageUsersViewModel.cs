using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using PetCare.Model;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class ManageUsersViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial ObservableCollection<UserAccount> Users { get; set; } = new();

        [ObservableProperty]
        public partial ObservableCollection<UserAccount> FilteredUsers { get; set; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial int TotalUsersCount { get; set; }

        [ObservableProperty]
        public partial int TotalAdminsCount { get; set; }

        [ObservableProperty]
        public partial int TotalClientsCount { get; set; }

        [ObservableProperty]
        public partial int ProtectedAccountsCount { get; set; }

        public ManageUsersViewModel(DatabaseService databaseService)
        {
            Title = "Manage Users";
            _databaseService = databaseService;
        }

        partial void OnSearchTextChanged(string value)
        {
            Search();
        }

        [RelayCommand]
        public async Task LoadUsersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userList = await _databaseService.GetUsersAsync();
                Users = new ObservableCollection<UserAccount>(userList);
                Search();
                UpdateCounts();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load users: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredUsers = new ObservableCollection<UserAccount>(Users);
            }
            else
            {
                var term = SearchText.Trim();
                var filtered = Users.Where(u =>
                    u.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    u.MobileNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    u.Role.Contains(term, StringComparison.OrdinalIgnoreCase));
                FilteredUsers = new ObservableCollection<UserAccount>(filtered);
            }
        }

        [RelayCommand]
        public async Task DeleteUserAsync(UserAccount user)
        {
            if (user == null) return;

            // PROTECTION RULE: Cannot delete Admin
            if (user.Role == "Admin")
            {
                await Shell.Current.DisplayAlertAsync("Protected Account", 
                    "This is a System Administration account and cannot be deleted.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlertAsync("Confirm Delete", 
                $"Are you sure you want to delete {user.FullName}?", "Yes", "No");

            if (confirm)
            {
                var result = user.Id > 0
                    ? await _databaseService.DeleteUserByIdAsync(user.Id)
                    : 0;

                if (result <= 0)
                {
                    result = await _databaseService.DeleteUserByEmailAsync(user.Email);
                }

                if (result > 0)
                {
                    await Shell.Current.DisplayAlertAsync("Success", $"{user.FullName} has been deleted successfully.", "OK");
                    await LoadUsersAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Delete Failed", "User could not be deleted. Please refresh and try again.", "OK");
                }
            }
        }

        private void UpdateCounts()
        {
            TotalUsersCount = Users.Count;
            TotalAdminsCount = Users.Count(u => u.Role == "Admin");
            TotalClientsCount = Users.Count(u => u.Role == "Client");
            ProtectedAccountsCount = TotalAdminsCount; // Currently only Admins are protected
        }

        [RelayCommand]
        public async Task EditUserAsync(UserAccount user)
        {
            if (user == null) return;

            // PROTECTION RULE: Cannot edit Admin
            if (user.Role == "Admin")
            {
                await Shell.Current.DisplayAlertAsync("Protected Account", 
                    "This is a System Administration account and cannot be modified.", "OK");
                return;
            }

            bool confirmEdit = await Shell.Current.DisplayAlertAsync("Confirm Edit", 
                $"You are about to edit {user.FullName}. Do you want to proceed?", "Yes", "Cancel");

            if (!confirmEdit) return;

            var newFullName = await Shell.Current.DisplayPromptAsync(
                "Edit User",
                "Update full name:",
                "Save",
                "Cancel",
                initialValue: user.FullName,
                maxLength: 80);

            if (newFullName == null) return;

            var newEmail = await Shell.Current.DisplayPromptAsync(
                "Edit User",
                "Update email:",
                "Save",
                "Cancel",
                initialValue: user.Email,
                keyboard: Keyboard.Email,
                maxLength: 120);

            if (newEmail == null) return;

            var newMobile = await Shell.Current.DisplayPromptAsync(
                "Edit User",
                "Update mobile number:",
                "Save",
                "Cancel",
                initialValue: user.MobileNumber,
                keyboard: Keyboard.Telephone,
                maxLength: 20);

            if (newMobile == null) return;

            newFullName = newFullName.Trim();
            newEmail = newEmail.Trim();
            newMobile = newMobile.Trim();

            if (string.IsNullOrWhiteSpace(newFullName) || newFullName.Length < 2)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please enter a valid full name.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please enter a valid email.", "OK");
                return;
            }

            if (!string.Equals(newEmail, user.Email, StringComparison.OrdinalIgnoreCase) && await _databaseService.IsEmailTakenAsync(newEmail))
            {
                await Shell.Current.DisplayAlertAsync("Validation", "This email is already in use.", "OK");
                return;
            }

            var updatedUser = new UserAccount
            {
                Id = user.Id,
                FullName = newFullName,
                Email = newEmail,
                MobileNumber = newMobile,
                Password = user.Password,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            var result = await _databaseService.UpdateClientUserAsync(updatedUser);
            if (result > 0)
            {
                await Shell.Current.DisplayAlertAsync("Success", "User updated successfully.", "OK");
                await LoadUsersAsync();
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Update Failed", "User could not be updated. Please refresh and try again.", "OK");
            }
        }

        [RelayCommand]
        public async Task ViewUserAsync(UserAccount user)
        {
            if (user == null) return;

            string details = $"Full Name: {user.FullName}\n" +
                             $"Email: {user.Email}\n" +
                             $"Phone: {user.MobileNumber}\n" +
                             $"Role: {user.Role}\n" +
                             $"Created: {user.CreatedAt:f}\n" +
                             $"Status: Active";

            await Shell.Current.DisplayAlert("User Profile Details", details, "Close");
        }
    }
}
