using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthService _authService;

    public ProfilePage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        _authService.Logout();
        Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}