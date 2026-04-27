using PetCare.ViewModel;

namespace PetCare.Page.Admin;

public partial class ManageAppointments : ContentPage
{
    private readonly ManageAppointmentsViewModel _viewModel;

	public ManageAppointments(ManageAppointmentsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadAppointmentsAsync();
    }
}
