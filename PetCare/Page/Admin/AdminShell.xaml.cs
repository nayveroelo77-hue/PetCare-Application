using PetCare.ViewModel;
using PetCare.Page.Admin.ManagePet;


namespace PetCare.Page.Admin;

public partial class AdminShell : Shell
{
	public AdminShell(AdminDashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

		Routing.RegisterRoute("ManagePets", typeof(ManagePet.ManagePetsPage));
		Routing.RegisterRoute("ManageAppointments", typeof(ManageAppointments));
		Routing.RegisterRoute("ManagePets/Add", typeof(ManagePet.AddPetPage));
		Routing.RegisterRoute("ManagePets/View", typeof(ManagePet.ViewPetPage));
		Routing.RegisterRoute("ManagePets/Edit", typeof(ManagePet.EditPetPage));

	}

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        if (BindingContext is AdminDashboardViewModel vm)
        {
            // Normalize route to the last segment for easier UI matching
            var location = args.Current.Location.ToString();
            vm.CurrentRoute = location.Split('/').LastOrDefault() ?? "AdminDashboard";
        }
    }
}
