using CommunityToolkit.Maui.Alerts;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class TournamentPage : ContentPage
{
    private readonly ICacheService _authService;

    public TournamentPage(ICacheService authService)
    {
        InitializeComponent();
        this._authService = authService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _authService.GetCurrentUserCredentialAsync() != null)
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(ListingPage)}");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }
}