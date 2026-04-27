using PetCare.ViewModel;

namespace PetCare.Page.Client;

public partial class ClientShell : Shell
{
	public ClientShell(ClientDashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        Routing.RegisterRoute("AddPetEntry", typeof(ClientPetDetailPage));
        Routing.RegisterRoute("EditPetEntry", typeof(ClientPetDetailPage));
        Routing.RegisterRoute("HealthRecord", typeof(PetHealthRecordPage));
        Routing.RegisterRoute("BookAppointment", typeof(BookAppointmentPage));
	}
}
