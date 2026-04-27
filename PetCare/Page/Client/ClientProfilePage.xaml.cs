using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class ClientProfilePage : ContentPage
{
    private readonly ClientProfileViewModel _viewModel;

    public ClientProfilePage(ClientProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadProfileAsync();
    }
}
