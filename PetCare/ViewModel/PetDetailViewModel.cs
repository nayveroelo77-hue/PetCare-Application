using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using PetCare.Model;
using PetCare.Service;

namespace PetCare.ViewModel
{
    public partial class PetDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        [ObservableProperty]
        public partial Pet Pet { get; set; } = new();

        [ObservableProperty]
        public partial int PetId { get; set; }

        [ObservableProperty]
        public partial string? SelectedSpecies { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<UserAccount> Clients { get; set; } = new();

        [ObservableProperty]
        public partial UserAccount? SelectedClient { get; set; }

        [ObservableProperty]
        public partial bool IsReadOnly { get; set; }

        [ObservableProperty]
        public partial bool IsEditMode { get; set; }

        [ObservableProperty]
        public partial string SaveButtonText { get; set; } = "Save Pet Record";

        [ObservableProperty]
        public partial bool IsClientMode { get; set; }

        public bool SuccessfullySaved { get; private set; }

        public List<string> SpeciesList { get; } = new() { "Dog", "Cat", "Bird", "Rabbit", "Other" };

        public PetDetailViewModel(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
            
            IsClientMode = _authService.UserRole == "Client";
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0) return;

            if (query.TryGetValue("Pet", out var petObj) && petObj is Pet pet)
            {
                Pet = pet;
                if (pet.Id > 0)
                {
                    PetId = pet.Id;
                }
            }

            // Handle PetId from URI string or Dictionary object
            if (query.TryGetValue("PetId", out var idObj))
            {
                var idStr = idObj.ToString()?.Trim();
                if (int.TryParse(idStr, out int id))
                {
                    PetId = id;
                }
            }

            // Handle IsReadOnly from URI string or Dictionary object
            if (query.TryGetValue("IsReadOnly", out var roObj))
            {
                var roStr = roObj.ToString()?.Trim()?.ToLower();
                if (bool.TryParse(roStr, out bool readOnly))
                {
                    IsReadOnly = readOnly;
                }
            }
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var clients = await _databaseService.GetClientsAsync();
                Clients = new ObservableCollection<UserAccount>(clients);

                var hasPreloadedPet = Pet != null && (!string.IsNullOrWhiteSpace(Pet.Name) || Pet.OwnerId > 0 || !string.IsNullOrWhiteSpace(Pet.Species) || !string.IsNullOrWhiteSpace(Pet.Breed));

                if (PetId > 0 || hasPreloadedPet)
                {
                    if (PetId > 0)
                    {
                        Pet = await _databaseService.GetPetAsync(PetId) ?? Pet ?? new Pet();
                    }
                    
                    if (Pet != null && Pet.Id > 0)
                    {
                        SelectedSpecies = SpeciesList.FirstOrDefault(s => s.Equals(Pet.Species, StringComparison.OrdinalIgnoreCase));
                        SelectedClient = Clients.FirstOrDefault(c => c.Id == Pet.OwnerId);
                        
                        if (IsReadOnly)
                        {
                            Title = "Pet Details";
                            SaveButtonText = "Close View";
                            IsEditMode = false;
                        }
                        else
                        {
                            IsEditMode = true;
                            Title = "Edit Pet Record";
                            SaveButtonText = "Update Pet Record";
                        }
                    }
                    else if (hasPreloadedPet)
                    {
                        SelectedSpecies = SpeciesList.FirstOrDefault(s => s.Equals(Pet.Species, StringComparison.OrdinalIgnoreCase));
                        SelectedClient = Clients.FirstOrDefault(c => c.Id == Pet.OwnerId);

                        if (IsReadOnly)
                        {
                            Title = "Pet Details";
                            SaveButtonText = "Close View";
                            IsEditMode = false;
                        }
                        else
                        {
                            IsEditMode = true;
                            Title = "Edit Pet Record";
                            SaveButtonText = "Update Pet Record";
                        }
                    }
                }
                else
                {
                    IsEditMode = false;
                    IsReadOnly = false;
                    Title = "Add New Pet";
                    SaveButtonText = "Save Pet Record";
                    Pet = new Pet();
                    SelectedSpecies = null;
                    SelectedClient = null;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load data: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsReadOnly)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            if (string.IsNullOrWhiteSpace(Pet.Name))
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please enter a pet name.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedSpecies))
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please select a species.", "OK");
                return;
            }

            if (SelectedClient == null && !IsClientMode)
            {
                await Shell.Current.DisplayAlertAsync("Validation", "Please select an owner.", "OK");
                return;
            }

            Pet.Species = SelectedSpecies;
            
            if (IsClientMode)
            {
                Pet.OwnerId = _authService.UserId;
            }
            else
            {
                Pet.OwnerId = SelectedClient!.Id;
            }

            try
            {
                if (IsEditMode)
                {
                    await _databaseService.UpdatePetAsync(Pet);
                    await Shell.Current.DisplayAlertAsync("Success", "Pet record updated successfully.", "OK");
                }
                else
                {
                    await _databaseService.SavePetAsync(Pet);
                    await Shell.Current.DisplayAlertAsync("Success", "Pet record added successfully.", "OK");
                }

                SuccessfullySaved = true;
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save pet: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        public async Task GoToEditAsync()
        {
            await Shell.Current.GoToAsync($"ManagePets/Edit?PetId={Pet.Id}");
        }
    }
}


