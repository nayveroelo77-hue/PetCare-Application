using PetCare.Page;
using PetCare.Page.Admin;
using PetCare.Page.Client;

namespace PetCare
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterPage", typeof(PetCare.Page.RegisterPage));

            CheckSession();
        }

        private async void CheckSession()
        {
            // Use Preferences directly for simplicity in startup logic
            bool isLoggedIn = Preferences.Default.Get("IsLoggedIn", false);
            string role = Preferences.Default.Get("UserRole", "");

            if (isLoggedIn && !string.IsNullOrEmpty(role))
            {
                // Delay slightly to ensure shell is ready
                await Task.Delay(100);
                if (role == "Admin")
                {
                    await Shell.Current.GoToAsync("//AdminDashboard");
                }
                else if (role == "Client")
                {
                    await Shell.Current.GoToAsync("//ClientDashboard");
                }
            }
        }
    }
}
