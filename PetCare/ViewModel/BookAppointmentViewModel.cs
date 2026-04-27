using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class BookAppointmentViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public ObservableCollection<Pet> MyPets { get; set; } = new();

        [ObservableProperty]
        public partial Pet? SelectedPet { get; set; }

        [ObservableProperty]
        public partial string? SelectedService { get; set; }

        [ObservableProperty]
        public partial DateTime AppointmentDate { get; set; } = DateTime.Now.Date.AddDays(1);

        [ObservableProperty]
        public partial TimeSpan AppointmentTime { get; set; } = new TimeSpan(9, 0, 0);

        [ObservableProperty]
        public partial string Notes { get; set; } = string.Empty;

        public DateTime MinDate { get; } = DateTime.Now.Date;

        public List<string> ServiceTypes { get; } = new()
        {
            "Checkup", "Grooming", "Vaccination", "Dental Cleaning",
            "Surgery", "Deworming", "Flea Treatment", "Other"
        };

        public BookAppointmentViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "Book Appointment";
        }

        [RelayCommand]
        public async Task LoadPetsAsync()
        {
            try
            {
                var pets = await _databaseService.GetPetsByOwnerIdAsync(_authService.UserId);
                MyPets.Clear();
                foreach (var pet in pets) MyPets.Add(pet);
                if (MyPets.Count == 1) SelectedPet = MyPets[0];
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load your pets.", "OK");
            }
        }

        [RelayCommand]
        public async Task BookAsync()
        {
            if (SelectedPet == null)
            {
                await Shell.Current.DisplayAlert("Validation", "Please select a pet.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedService))
            {
                await Shell.Current.DisplayAlert("Validation", "Please select a service type.", "OK");
                return;
            }

            var appointmentDateTime = AppointmentDate.Date + AppointmentTime;
            if (appointmentDateTime <= DateTime.Now)
            {
                await Shell.Current.DisplayAlert("Validation", "Appointment must be in the future.", "OK");
                return;
            }

            try
            {
                var appointment = new Appointment
                {
                    PetId = SelectedPet.Id,
                    DateTime = appointmentDateTime,
                    ServiceType = SelectedService,
                    Status = "Scheduled",
                    Notes = Notes
                };

                await _databaseService.SaveAppointmentAsync(appointment);
                await Shell.Current.DisplayAlert("Success", "Appointment booked successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to book appointment.", "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
