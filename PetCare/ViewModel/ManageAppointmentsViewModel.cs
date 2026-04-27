using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class ManageAppointmentsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<AppointmentDisplay> AllAppointments { get; set; } = new();
        public ObservableCollection<AppointmentDisplay> FilteredAppointments { get; set; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; }

        [ObservableProperty]
        public partial int TotalAppointments { get; set; }

        [ObservableProperty]
        public partial int PendingCount { get; set; }

        [ObservableProperty]
        public partial int ApprovedCount { get; set; }

        [ObservableProperty]
        public partial int RejectedCount { get; set; }

        public ManageAppointmentsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Manage Appointments";
        }

        [RelayCommand]
        public async Task LoadAppointmentsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var appointments = await _databaseService.GetAppointmentsAsync();
                var pets = await _databaseService.GetPetsAsync();
                var clients = await _databaseService.GetClientsAsync();

                AllAppointments.Clear();
                foreach (var app in appointments)
                {
                    var pet = pets.FirstOrDefault(p => p.Id == app.PetId);
                    var owner = clients.FirstOrDefault(c => c.Id == (pet?.OwnerId ?? 0));

                    AllAppointments.Add(new AppointmentDisplay
                    {
                        Appointment = app,
                        PetName = pet?.Name ?? "Unknown Patient",
                        OwnerName = owner?.FullName ?? "Unknown Owner"
                    });
                }

                CalculateStats();
                FilterAppointments();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load appointments. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalculateStats()
        {
            TotalAppointments = AllAppointments.Count;
            PendingCount = AllAppointments.Count(a => a.Appointment.Status == "Scheduled" || a.Appointment.Status == "Pending");
            ApprovedCount = AllAppointments.Count(a => a.Appointment.Status == "Approved" || a.Appointment.Status == "Completed");
            RejectedCount = AllAppointments.Count(a => a.Appointment.Status == "Rejected");
        }

        [RelayCommand]
        private void FilterAppointments()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredAppointments.Clear();
                foreach (var app in AllAppointments) FilteredAppointments.Add(app);
            }
            else
            {
                var query = SearchText.ToLower();
                var results = AllAppointments.Where(a => 
                    a.PetName.ToLower().Contains(query) || 
                    a.OwnerName.ToLower().Contains(query) || 
                    a.Appointment.ServiceType.ToLower().Contains(query) ||
                    a.Appointment.Status.ToLower().Contains(query)).ToList();

                FilteredAppointments.Clear();
                foreach (var res in results) FilteredAppointments.Add(res);
            }
        }

        partial void OnSearchTextChanged(string value) => FilterAppointments();

        [RelayCommand]
        private async Task ApproveAppointmentAsync(AppointmentDisplay app)
        {
            if (app == null) return;
            
            var result = await _databaseService.UpdateAppointmentStatusAsync(app.Appointment.Id, "Approved");
            if (result > 0)
            {
                await Shell.Current.DisplayAlert("Success", "Appointment approved successfully.", "OK");
                await LoadAppointmentsAsync();
            }
        }

        [RelayCommand]
        private async Task RejectAppointmentAsync(AppointmentDisplay app)
        {
            if (app == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmation", "Are you sure you want to reject this appointment?", "Yes", "Cancel");
            if (!confirm) return;

            var result = await _databaseService.UpdateAppointmentStatusAsync(app.Appointment.Id, "Rejected");
            if (result > 0)
            {
                await Shell.Current.DisplayAlert("Success", "Appointment rejected successfully.", "OK");
                await LoadAppointmentsAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteAppointmentAsync(AppointmentDisplay app)
        {
            if (app == null) return;

            // Protection logic: Approved appointments cannot be deleted
            if (app.Appointment.Status == "Approved")
            {
                await Shell.Current.DisplayAlert("Safety Protection", "Approved appointments cannot be deleted.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert("Confirmation", "Are you sure you want to delete this pending appointment?", "Yes", "Cancel");
            if (!confirm) return;

            var result = await _databaseService.DeleteAppointmentAsync(app.Appointment.Id);
            if (result > 0)
            {
                await Shell.Current.DisplayAlert("Success", "Appointment deleted successfully.", "OK");
                await LoadAppointmentsAsync();
            }
        }

        [RelayCommand]
        private async Task ViewAppointmentAsync(AppointmentDisplay app)
        {
            if (app == null) return;

            string details = $"Patient: {app.PetName}\n" +
                             $"Owner: {app.OwnerName}\n" +
                             $"Service: {app.Appointment.ServiceType}\n" +
                             $"Date: {app.Appointment.DateTime:MMM dd, yyyy}\n" +
                             $"Time: {app.Appointment.DateTime:hh:mm tt}\n" +
                             $"Status: {app.Appointment.Status}\n\n" +
                             $"Notes: {app.Appointment.Notes ?? "No notes available."}";

            await Shell.Current.DisplayAlert("Appointment Details", details, "Close");
        }

        [RelayCommand]
        private async Task CompleteAppointmentAsync(AppointmentDisplay app)
        {
            if (app == null) return;

            string notes = await Shell.Current.DisplayPromptAsync(
                "Treatment Notes", 
                $"Enter medical notes/treatment details for {app.PetName}'s visit:", 
                "Complete", "Cancel", "e.g., Vaccinated, prescribed meds...");

            if (notes == null) return; // User cancelled

            var result = await _databaseService.CompleteAppointmentWithNotesAsync(app.Appointment.Id, notes);
            if (result > 0)
            {
                await Shell.Current.DisplayAlert("Completed", "Appointment has been finalized and treatment notes recorded.", "OK");
                await LoadAppointmentsAsync();
            }
        }
    }
}
