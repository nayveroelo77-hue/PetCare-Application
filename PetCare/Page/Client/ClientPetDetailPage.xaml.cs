using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class ClientPetDetailPage : ContentPage
{
    private readonly PetDetailViewModel _viewModel;

	public ClientPetDetailPage(PetDetailViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadDataAsync();
    }
}
