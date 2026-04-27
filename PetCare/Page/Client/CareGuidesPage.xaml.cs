using PetCare.ViewModel;

namespace PetCare.Page.Client
{
    public partial class CareGuidesPage : ContentPage
    {
        public CareGuidesPage(CareGuidesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
