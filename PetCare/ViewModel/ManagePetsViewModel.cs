using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using PetCare.Model;
using PetCare.Service;
using PetCare.Page.Admin;

namespace PetCare.ViewModel
{
    public partial class ManagePetsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public partial ObservableCollection<PetDisplay> Pets { get; set; } = new();

        [ObservableProperty]
        public partial ObservableCollection<PetDisplay> FilteredPets { get; set; } = new();

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SelectedSpecies { get; set; }

        [ObservableProperty]
        public partial int TotalPetsCount { get; set; }

        [ObservableProperty]
        public partial int DogCount { get; set; }

        [ObservableProperty]
        public partial int CatCount { get; set; }

        [ObservableProperty]
        public partial int OtherCount { get; set; }

        public List<string> SpeciesList { get; } = new() { "All", "Dog", "Cat", "Other" };

        public ManagePetsViewModel(DatabaseService databaseService)
        {
            Title = "Manage Pets";
            _databaseService = databaseService;
            SelectedSpecies = "All";
        }

        [RelayCommand]
        public async Task LoadPetsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var petList = await _databaseService.GetPetsAsync();
                var clientList = await _databaseService.GetClientsAsync();

                var displayList = petList.Select(p => new PetDisplay
                {
                    Pet = p,
                    OwnerName = clientList.FirstOrDefault(c => c.Id == p.OwnerId)?.FullName ?? "Unknown Owner",
                    OwnerEmail = clientList.FirstOrDefault(c => c.Id == p.OwnerId)?.Email ?? "N/A"
                }).ToList();

                Pets = new ObservableCollection<PetDisplay>(displayList);
                Filter();
                UpdateStats();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load pets: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Filter()
        {
            var filtered = Pets.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => 
                    p.Pet.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                    p.Pet.Breed.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.OwnerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedSpecies != "All")
            {
                if (SelectedSpecies == "Other")
                {
                    filtered = filtered.Where(p => p.Pet.Species != "Dog" && p.Pet.Species != "Cat");
                }
                else
                {
                    filtered = filtered.Where(p => p.Pet.Species == SelectedSpecies);
                }
            }

            FilteredPets = new ObservableCollection<PetDisplay>(filtered);
        }

        private void UpdateStats()
        {
            TotalPetsCount = Pets.Count;
            DogCount = Pets.Count(p => p.Pet.Species == "Dog");
            CatCount = Pets.Count(p => p.Pet.Species == "Cat");
            OtherCount = TotalPetsCount - DogCount - CatCount;
        }

        [RelayCommand]
        public async Task DeletePetAsync(PetDisplay petDisplay)
        {
            if (petDisplay == null) return;

            bool confirm = await Shell.Current.DisplayAlertAsync("Confirm Delete", 
                $"Are you sure you want to delete {petDisplay.Pet.Name}?", "Yes", "No");

            if (confirm)
            {
                var petId = petDisplay.Pet.Id;

                if (petId <= 0)
                {
                    var currentPets = await _databaseService.GetPetsAsync();
                    var matchedPet = currentPets.FirstOrDefault(p =>
                        p.Name == petDisplay.Pet.Name &&
                        p.Breed == petDisplay.Pet.Breed &&
                        p.OwnerId == petDisplay.Pet.OwnerId);

                    petId = matchedPet?.Id ?? 0;
                }

                var result = petId > 0
                    ? await _databaseService.DeletePetByIdAsync(petId)
                    : 0;

                if (result <= 0)
                {
                    result = await _databaseService.DeletePetByDetailsAsync(petDisplay.Pet);
                }

                if (result > 0)
                {
                    await Shell.Current.DisplayAlertAsync("Success", $"{petDisplay.Pet.Name} record deleted successfully.", "OK");
                    await LoadPetsAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Delete Failed", "Pet record could not be deleted. Please refresh and try again.", "OK");
                }
            }
        }


        [RelayCommand]
        public async Task AddPetAsync()
        {
            await ShowPetFormAsync(null);
        }

        [RelayCommand]
        public async Task ViewPetAsync(PetDisplay petDisplay)
        {
            if (petDisplay == null) return;

            var pet = petDisplay.Pet;
            string details = $"Name: {pet.Name}\n" +
                             $"Species: {pet.Species}\n" +
                             $"Breed: {pet.Breed}\n" +
                             $"Age: {pet.Age} years\n" +
                             $"Weight: {pet.Weight} kg\n" +
                             $"Owner: {petDisplay.OwnerName}\n" +
                             $"Email: {petDisplay.OwnerEmail}\n" +
                             $"Added: {pet.CreatedAt:d}";

            await Shell.Current.DisplayAlert($"{pet.Name}'s Profile", details, "Close");
        }

        [RelayCommand]
        public async Task EditPetAsync(PetDisplay petDisplay)
        {
            if (petDisplay == null) return;
            await ShowPetFormAsync(petDisplay.Pet);
        }

        private async Task ShowPetFormAsync(Pet? existingPet)
        {
            bool isEdit = existingPet != null;
            string action = isEdit ? "Update" : "Add";
            var pet = isEdit ? existingPet! : new Pet();

            // 1. Name
            var name = await Shell.Current.DisplayPromptAsync($"{action} Pet", "Enter Pet Name:", initialValue: pet.Name);
            if (string.IsNullOrWhiteSpace(name)) return;

            // 2. Species
            var species = await Shell.Current.DisplayActionSheet("Select Species", "Cancel", null, "Dog", "Cat", "Bird", "Rabbit", "Other");
            if (species == "Cancel" || string.IsNullOrWhiteSpace(species)) return;

            // 3. Breed
            var breed = await Shell.Current.DisplayPromptAsync($"{action} Pet", "Enter Breed:", initialValue: pet.Breed);
            if (breed == null) return; 

            // 4. Age
            var ageStr = await Shell.Current.DisplayPromptAsync($"{action} Pet", "Enter Age:", initialValue: pet.Age.ToString(), keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(ageStr) || !int.TryParse(ageStr, out int age)) age = 0;

            // 5. Weight
            var weightStr = await Shell.Current.DisplayPromptAsync($"{action} Pet", "Enter Weight (kg):", initialValue: pet.Weight.ToString(), keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(weightStr) || !double.TryParse(weightStr, out double weight)) weight = 0;

            // 6. Owner
            var clients = await _databaseService.GetClientsAsync();
            var clientNames = clients.Select(c => c.FullName).ToArray();
            var ownerName = await Shell.Current.DisplayActionSheet("Select Owner", "Cancel", null, clientNames);
            
            if (ownerName == "Cancel" || string.IsNullOrWhiteSpace(ownerName)) return;
            var selectedOwner = clients.FirstOrDefault(c => c.FullName == ownerName);
            if (selectedOwner == null) return;

            pet.Name = name;
            pet.Species = species;
            pet.Breed = breed;
            pet.Age = age;
            pet.Weight = weight;
            pet.OwnerId = selectedOwner.Id;

            if (isEdit)
                await _databaseService.UpdatePetAsync(pet);
            else
                await _databaseService.SavePetAsync(pet);

            await Shell.Current.DisplayAlert("Success", $"Pet {name} {action.ToLower()}ed successfully.", "OK");
            await LoadPetsAsync();
        }
    }

    public class PetDisplay
    {
        public Pet Pet { get; set; } = new();
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public string PetTypeIcon => Pet.Species.ToLower() switch
        {
            "dog" => "dog_icon.jpg",
            "cat" => "cat_icon.jpg",
            _ => "other_pet_icon.jpg"
        };
    }
}
