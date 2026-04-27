using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class PetHealthViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial Pet Pet { get; set; } = new();

        [ObservableProperty]
        public partial string WeightDisplay { get; set; } = "N/A";

        [ObservableProperty]
        public partial int CompletedVisits { get; set; }

        [ObservableProperty]
        public partial string LastVisitDate { get; set; } = "No visits yet";

        public ObservableCollection<AppointmentDisplay> HealthHistory { get; set; } = new();

        public PetHealthViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Health Records";
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Pet", out var petObj) && petObj is Pet pet)
            {
                Pet = pet;
                Title = $"{Pet.Name}'s Health Records";
                WeightDisplay = $"{Pet.Weight} kg";
            }
        }

        [RelayCommand]
        public async Task LoadHealthHistoryAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var allAppointments = await _databaseService.GetAppointmentsAsync();
                var petHistory = allAppointments
                    .Where(a => a.PetId == Pet.Id)
                    .OrderByDescending(a => a.DateTime)
                    .ToList();

                CompletedVisits = petHistory.Count(a => a.Status == "Completed");
                var lastVisit = petHistory.FirstOrDefault(a => a.Status == "Completed");
                LastVisitDate = lastVisit != null ? lastVisit.DateTime.ToString("MMM dd, yyyy") : "No visits yet";

                HealthHistory.Clear();
                foreach (var record in petHistory)
                {
                    HealthHistory.Add(new AppointmentDisplay
                    {
                        Appointment = record,
                        PetName = Pet.Name,
                        OwnerName = string.Empty
                    });
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load health records.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
