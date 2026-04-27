using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class ClientDashboard : ContentPage
{
    private readonly ClientDashboardViewModel _viewModel;

	public ClientDashboard(ClientDashboardViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadDashboardDataAsync();
    }

    private async void OnNewAppointmentClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("BookAppointment");
    }
}
