using PetCare.ViewModel;

namespace PetCare.Page.Admin;

public partial class ManageUsers : ContentPage
{
    private readonly ManageUsersViewModel _viewModel;

    public ManageUsers(ManageUsersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUsersCommand.ExecuteAsync(null);
    }

    public async Task<bool> ConfirmDeleteUserAsync()
    {
        return await DisplayAlert("Confirmation", "Are you sure you want to delete this user?", "Yes", "Cancel");
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
