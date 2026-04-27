using PetCare.Page;
using PetCare.Page.Admin;
using PetCare.Page.Client;
using PetCare.ViewModel;
using PetCare.Service;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace PetCare
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            
            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<AuthService>();
            
            // ViewModels
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<AdminDashboardViewModel>();
            builder.Services.AddTransient<ManageUsersViewModel>();
            builder.Services.AddTransient<ManagePetsViewModel>();
            builder.Services.AddTransient<ManageAppointmentsViewModel>();
            builder.Services.AddTransient<PetDetailViewModel>();
            builder.Services.AddTransient<ClientDashboardViewModel>();
            builder.Services.AddTransient<MyPetsViewModel>();
            builder.Services.AddTransient<MyAppointmentsViewModel>();
            builder.Services.AddTransient<ClientProfileViewModel>();
            builder.Services.AddTransient<PetHealthViewModel>();
            builder.Services.AddTransient<BookAppointmentViewModel>();
            builder.Services.AddTransient<CareGuidesViewModel>();
            
            // Pages
            builder.Services.AddSingleton<PetCare.Page.LoginPage>();
            builder.Services.AddTransient<PetCare.Page.RegisterPage>();
            builder.Services.AddTransient<PetCare.Page.Admin.AdminDashboard>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManageUsers>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManagePet.ManagePetsPage>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManagePet.AddPetPage>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManagePet.ViewPetPage>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManagePet.EditPetPage>();
            builder.Services.AddTransient<PetCare.Page.Admin.ManageAppointments>();
            builder.Services.AddTransient<PetCare.Page.Client.ClientDashboard>();
            builder.Services.AddTransient<PetCare.Page.Client.MyPetsPage>();
            builder.Services.AddTransient<PetCare.Page.Client.ClientPetDetailPage>();
            builder.Services.AddTransient<PetCare.Page.Client.PetHealthRecordPage>();
            builder.Services.AddTransient<PetCare.Page.Client.BookAppointmentPage>();
            builder.Services.AddTransient<PetCare.Page.Client.MyAppointmentsPage>();
            builder.Services.AddTransient<PetCare.Page.Client.ClientProfilePage>();
            builder.Services.AddTransient<PetCare.Page.Client.CareGuidesPage>();

            return builder.Build();
        }
    }
}
