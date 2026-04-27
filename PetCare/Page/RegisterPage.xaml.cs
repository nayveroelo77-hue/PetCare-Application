using PetCare.ViewModel;
using System.Text.RegularExpressions;

namespace PetCare.Page;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // 1. Full Name Validation
        if (string.IsNullOrWhiteSpace(_viewModel.FullName))
        {
            await DisplayAlert("Full Name Required", "Please enter your full name.", "OK");
            return;
        }

        if (_viewModel.FullName.Trim().Length < 2)
        {
            await DisplayAlert("Full Name Too Short", "Full name must be at least 2 characters long.", "OK");
            return;
        }

        // 2. Email Validation
        if (string.IsNullOrWhiteSpace(_viewModel.Email))
        {
            await DisplayAlert("Email Required", "Please enter your email address.", "OK");
            return;
        }

        if (!IsValidEmail(_viewModel.Email))
        {
            await DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        // 3. Mobile Number Validation
        if (string.IsNullOrWhiteSpace(_viewModel.MobileNumber))
        {
            await DisplayAlert("Mobile Number Required", "Please enter your mobile number.", "OK");
            return;
        }

        if (!IsValidMobile(_viewModel.MobileNumber))
        {
            await DisplayAlert("Invalid Mobile Number", "Please enter a valid mobile number.", "OK");
            return;
        }

        // 4. Password Validation
        if (string.IsNullOrWhiteSpace(_viewModel.Password))
        {
            await DisplayAlert("Password Created Required", "Please create a password.", "OK");
            return;
        }

        if (_viewModel.Password.Length < 8)
        {
            await DisplayAlert("Password Too Short", "Password must be at least 8 characters long.", "OK");
            return;
        }

        // 5. Confirm Password Validation
        if (string.IsNullOrWhiteSpace(_viewModel.ConfirmPassword))
        {
            await DisplayAlert("Confirm Your Password", "Please confirm your password.", "OK");
            return;
        }

        if (_viewModel.Password != _viewModel.ConfirmPassword)
        {
            await DisplayAlert("Password Mismatch", "Password and confirm password do not match.", "OK");
            return;
        }

        // 6. Terms Validation
        if (!_viewModel.AgreeToTerms)
        {
            await DisplayAlert("Accept Terms", "Please accept the Terms and Privacy Policy before continuing.", "OK");
            return;
        }

        // 7. Perform Registration
        try 
        {
            // Execute the VM command to handle the actual service call and "email exists" logic
            await _viewModel.RegisterCommand.ExecuteAsync(null);
            
            // Note: The VM handles "An account with this email address already exists."
        }
        catch (Exception ex)
        {
            await DisplayAlert("Registration Error", "Something went wrong. Please try again later.", "OK");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidMobile(string mobile)
    {
        // Simple regex for mobile number - adjusts based on needs
        return Regex.IsMatch(mobile, @"^\+?[0-9\s-]{7,15}$");
    }
}
