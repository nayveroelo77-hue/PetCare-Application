using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class MyAppointmentsPage : ContentPage
{
    private readonly MyAppointmentsViewModel _viewModel;

	public MyAppointmentsPage(MyAppointmentsViewModel viewModel)
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
