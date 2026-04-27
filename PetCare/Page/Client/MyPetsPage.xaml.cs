using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class MyPetsPage : ContentPage
{
    private readonly MyPetsViewModel _viewModel;

	public MyPetsPage(MyPetsViewModel viewModel)
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
