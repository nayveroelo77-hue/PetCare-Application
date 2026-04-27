using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class MyAppointmentsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public ObservableCollection<AppointmentDisplay> ClientAppointments { get; set; } = new();

        public MyAppointmentsViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "My Appointments";
        }

        [RelayCommand]
        public async Task LoadAppointmentsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = _authService.UserId;
                var pets = await _databaseService.GetPetsByOwnerIdAsync(userId);
                var appointments = await _databaseService.GetAppointmentsByOwnerIdAsync(userId);

                ClientAppointments.Clear();
                foreach (var app in appointments)
                {
                    var pet = pets.FirstOrDefault(p => p.Id == app.PetId);
                    ClientAppointments.Add(new AppointmentDisplay
                    {
                        Appointment = app,
                        PetName = pet?.Name ?? "My Pet",
                        OwnerName = _authService.UserName
                    });
                }
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

        [RelayCommand]
        public async Task CancelAppointmentAsync(AppointmentDisplay item)
        {
            if (item == null) return;

            bool answer = await Shell.Current.DisplayAlert(
                "Cancel Appointment",
                $"Cancel {item.Appointment.ServiceType} for {item.PetName} on {item.Appointment.DateTime:MMM dd}?",
                "Yes, Cancel", "No");

            if (!answer) return;

            try
            {
                await _databaseService.CancelAppointmentAsync(item.Appointment.Id);
                await LoadAppointmentsAsync();
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to cancel appointment.", "OK");
            }
        }

        [RelayCommand]
        private async Task ViewAppointmentAsync(AppointmentDisplay item)
        {
            if (item == null) return;

            string details = $"Patient: {item.PetName}\n" +
                             $"Service: {item.Appointment.ServiceType}\n" +
                             $"Date: {item.Appointment.DateTime:MMM dd, yyyy}\n" +
                             $"Time: {item.Appointment.DateTime:hh:mm tt}\n" +
                             $"Status: {item.Appointment.Status}\n\n" +
                             $"Notes: {item.Appointment.Notes ?? "No notes available."}";

            await Shell.Current.DisplayAlert("Appointment Details", details, "Close");
        }

        [RelayCommand]
        private async Task BookNewVisitAsync()
        {
            await Shell.Current.GoToAsync("BookAppointment");
        }
    }
}
