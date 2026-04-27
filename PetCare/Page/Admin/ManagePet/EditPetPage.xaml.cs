using PetCare.ViewModel;

namespace PetCare.Page.Admin.ManagePet;

public partial class EditPetPage : ContentPage
{
	public EditPetPage(PetDetailViewModel viewModel)
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
            vm.IsReadOnly = false;
            vm.IsEditMode = true;
            _ = vm.LoadDataAsync();
        }
    }
}
