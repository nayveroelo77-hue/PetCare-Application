using PetCare.ViewModel;

namespace PetCare.Page.Admin.ManagePet;

public partial class ManagePetsPage : ContentPage
{
    private readonly ManagePetsViewModel _viewModel;

	public ManagePetsPage(ManagePetsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadPetsAsync();
    }
}
