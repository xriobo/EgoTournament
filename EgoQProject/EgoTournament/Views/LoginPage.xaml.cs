using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _authService;

    public LoginPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _authService.Login();
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
}