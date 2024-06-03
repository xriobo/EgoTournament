using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();

        BindingContext = loginViewModel;
    }
}