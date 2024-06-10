using CommunityToolkit.Maui.Alerts;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class ListingPage : ContentPage
{
    private readonly ICacheService _cacheService;

    public ListingPage(ICacheService cacheService)
    {
        InitializeComponent();
        _cacheService = cacheService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var currentUserCredential = await _cacheService.GetCurrentUserCredentialAsync();
        if (currentUserCredential != null)
        {
            GetProfileInfo();
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

    private async void GetProfileInfo()
    {
        var currentUser = await _cacheService.GetCurrentUserAsync();
        UserEmail.Text = currentUser?.Email;
        SummonerName.Text = currentUser?.SummonerName;
    }
}