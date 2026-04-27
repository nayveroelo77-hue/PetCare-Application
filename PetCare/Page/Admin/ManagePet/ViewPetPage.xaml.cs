using PetCare.ViewModel;

namespace PetCare.Page.Admin.ManagePet;

public partial class ViewPetPage : ContentPage
{
	public ViewPetPage(PetDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is PetDetailViewModel vm)
        {
            vm.IsClientMode = false;
            vm.IsReadOnly = true;
            vm.IsEditMode = false;
            _ = vm.LoadDataAsync();
        }
    }
}
