using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class ClientDashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        public partial string WelcomeMessage { get; set; } = "Welcome Back!";

        [ObservableProperty]
        public partial int TotalPets { get; set; }

        [ObservableProperty]
        public partial int UpcomingAppointments { get; set; }

        [ObservableProperty]
        public partial int PendingAppointments { get; set; }

        public ObservableCollection<AppointmentDisplay> RecentVisits { get; set; } = new();

        public ClientDashboardViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "Dashboard";
            
            WelcomeMessage = $"Welcome Back, {_authService.UserName}!";
        }

        [RelayCommand]
        public async Task LoadDashboardDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = _authService.UserId;
                var pets = await _databaseService.GetPetsByOwnerIdAsync(userId);
                var appointments = await _databaseService.GetAppointmentsByOwnerIdAsync(userId);

                TotalPets = pets.Count;
                UpcomingAppointments = appointments.Count(a => a.DateTime >= DateTime.Now && a.Status == "Approved");
                PendingAppointments = appointments.Count(a => a.Status == "Scheduled" || a.Status == "Pending");

                RecentVisits.Clear();
                var latestAppointments = appointments.Take(5).ToList();
                foreach (var app in latestAppointments)
                {
                    var pet = pets.FirstOrDefault(p => p.Id == app.PetId);
                    RecentVisits.Add(new AppointmentDisplay
                    {
                        Appointment = app,
                        PetName = pet?.Name ?? "My Pet",
                        OwnerName = _authService.UserName
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load dashboard. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
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

        [RelayCommand]
        private async Task NavigateAsync(string route)
        {
            if (string.IsNullOrWhiteSpace(route)) return;
            await Shell.Current.GoToAsync(route);
        }
    }
}
