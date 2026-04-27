using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class AdminDashboardViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial string AdminName { get; set; }

        [ObservableProperty]
        public partial string CurrentRoute { get; set; } = "AdminDashboard";

        [ObservableProperty]
        public partial int TotalUsers { get; set; }

        [ObservableProperty]
        public partial int TotalPets { get; set; }

        [ObservableProperty]
        public partial int TotalAppointments { get; set; }

        [ObservableProperty]
        public partial string LastUpdated { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<ActivityItem> RecentActivities { get; set; } = new();
        public System.Collections.ObjectModel.ObservableCollection<ChartItem> MonthlyAppointments { get; set; } = new();

        public AdminDashboardViewModel(AuthService authService, DatabaseService databaseService)
        {
            Title = "Admin Dashboard";
            _authService = authService;
            _databaseService = databaseService;
            AdminName = _authService.UserName;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            TotalUsers = await _databaseService.GetTotalCountAsync<UserAccount>();
            TotalPets = await _databaseService.GetTotalCountAsync<Pet>();
            TotalAppointments = await _databaseService.GetTotalCountAsync<Appointment>();
            LastUpdated = DateTime.Now.ToString("g");

            // Fetch Real Chart Data
            var trends = await _databaseService.GetMonthlyAppointmentTrendsAsync();
            MonthlyAppointments.Clear();
            int maxVal = trends.Values.Any() ? trends.Values.Max() : 1;
            if (maxVal == 0) maxVal = 1;

            foreach (var trend in trends)
            {
                MonthlyAppointments.Add(new ChartItem 
                { 
                    Label = trend.Key, 
                    Value = trend.Value, 
                    Percentage = (double)trend.Value / maxVal 
                });
            }

            // Fetch Real Recent Activity
            var recentAppointments = await _databaseService.GetRecentAppointmentsAsync(5);
            RecentActivities.Clear();
            foreach (var app in recentAppointments)
            {
                RecentActivities.Add(new ActivityItem 
                { 
                    Title = $"{app.ServiceType} Appointment", 
                    Description = app.Status, 
                    Time = app.DateTime.ToString("MMM dd, HH:mm"), 
                    Icon = GetIconForService(app.ServiceType) 
                });
            }
        }

        private string GetIconForService(string service)
        {
            if (string.IsNullOrEmpty(service)) return "calendar_icon.png";

            return service.ToLower() switch
            {
                // Current appointment-related types
                "checkup" or "grooming" or "vaccination" or "appointment" => "calendar_icon.png",
                
                // For future-proofing (based on available assets)
                "pet" or "adoption" => "pets_icon.png",
                "user" or "account" or "profile" => "users_icon.png",
                
                _ => "calendar_icon.png"
            };
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

    public class ActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Icon { get; set; } = "paw.png";
    }

    public class ChartItem
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public double Percentage { get; set; } // 0.0 to 1.0 for HeightRequest scaling
    }
}
