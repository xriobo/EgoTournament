using CommunityToolkit.Maui.Alerts;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class MainPage : ContentPage
{
    private readonly IAuthService _authService;

    public MainPage(IAuthService authService)
    {
        InitializeComponent();
        this._authService = authService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _authService.GetCurrentAuthenticatedUserAsync() != null)
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You should Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }
}