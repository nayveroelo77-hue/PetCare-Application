using PetCare.ViewModel;

namespace PetCare.Page.Admin;

public partial class AdminDashboard : ContentPage
{
	private readonly AdminDashboardViewModel _viewModel;

	public AdminDashboard(AdminDashboardViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        try 
        {
            await _viewModel.LoadDataCommand.ExecuteAsync(null);
        }
        catch 
        {
            await DisplayAlert("System Error", "Unable to load dashboard data right now. Please try again.", "OK");
        }
    }
}
