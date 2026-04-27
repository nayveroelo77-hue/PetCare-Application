using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class PetHealthRecordPage : ContentPage
{
    private readonly PetHealthViewModel _viewModel;

    public PetHealthRecordPage(PetHealthViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadHealthHistoryAsync();
    }
}
