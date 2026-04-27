using PetCare.ViewModel;

namespace PetCare.Page.Admin.ManagePet;

public partial class AddPetPage : ContentPage
{
    private readonly PetDetailViewModel _viewModel;

    public AddPetPage(PetDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }

    public async Task<bool> ValidateInputsAsync()
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Pet.Name))
        {
            await DisplayAlert("Data Required", "Please enter the pet name.", "OK");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_viewModel.Pet.Species) || _viewModel.Pet.Species == "Select...")
        {
            await DisplayAlert("Data Required", "Please select a valid pet type.", "OK");
            return false;
        }

        return true;
    }

    public async Task ShowSuccessAsync(string message)
    {
        await DisplayAlert("Success", message, "OK");
    }

    public async Task ShowErrorAsync(string message)
    {
        await DisplayAlert("Error", message, "OK");
    }
}
