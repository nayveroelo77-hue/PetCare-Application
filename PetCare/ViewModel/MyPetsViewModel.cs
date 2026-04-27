using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetCare.Model;
using PetCare.Service;
using System.Collections.ObjectModel;

namespace PetCare.ViewModel
{
    public partial class MyPetsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public ObservableCollection<Pet> OwnerPets { get; set; } = new();

        public MyPetsViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            Title = "My Pets";
        }

        [RelayCommand]
        public async Task LoadPetsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var pets = await _databaseService.GetPetsByOwnerIdAsync(_authService.UserId);
                // get related appointments once to derive any admin notes for each pet
                var appointments = await _databaseService.GetAppointmentsByOwnerIdAsync(_authService.UserId);

                OwnerPets.Clear();
                foreach (var pet in pets)
                {
                    // find most recent appointment for this pet that has notes
                    var latest = appointments
                        .Where(a => a.PetId == pet.Id && !string.IsNullOrWhiteSpace(a.Notes))
                        .OrderByDescending(a => a.DateTime)
                        .FirstOrDefault();

                    pet.LatestNotes = latest?.Notes;
                    OwnerPets.Add(pet);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to load pets. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddPetAsync()
        {
            await Shell.Current.GoToAsync("AddPetEntry");
        }

        [RelayCommand]
        public async Task EditPetAsync(Pet pet)
        {
            if (pet == null) return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Pet", pet }
            };
            await Shell.Current.GoToAsync("EditPetEntry", navigationParameter);
        }

        [RelayCommand]
        public async Task DeletePetAsync(Pet pet)
        {
            if (pet == null) return;

            bool answer = await Shell.Current.DisplayAlert("Delete Pet", $"Are you sure you want to remove {pet.Name}?", "Yes", "No");
            if (!answer) return;

            try
            {
                await _databaseService.DeletePetAsync(pet);
                OwnerPets.Remove(pet);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete pet.", "OK");
            }
        }

        [RelayCommand]
        public async Task ViewHealthRecordAsync(Pet pet)
        {
            if (pet == null) return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Pet", pet }
            };
            await Shell.Current.GoToAsync("HealthRecord", navigationParameter);
        }
    }
}
