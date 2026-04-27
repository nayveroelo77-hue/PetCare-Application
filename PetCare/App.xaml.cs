using Microsoft.Extensions.DependencyInjection;
using PetCare.Service;
using PetCare.ViewModel;
using PetCare.Page.Admin;
using PetCare.Page.Client;

namespace PetCare
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var services = Current?.Handler?.MauiContext?.Services;
            
            if (services != null)
            {
                var authService = services.GetService<AuthService>();
                if (authService != null && authService.IsLoggedIn)
                {
                    if (authService.UserRole == "Admin")
                    {
                        var adminViewModel = services.GetRequiredService<AdminDashboardViewModel>();
                        return new Window(new AdminShell(adminViewModel));
                    }
                    else
                    {
                        var clientViewModel = services.GetRequiredService<ClientDashboardViewModel>();
                        return new Window(new ClientShell(clientViewModel));
                    }
                }
            }

            return new Window(new UnauthenticatedShell());
        }
    }
}