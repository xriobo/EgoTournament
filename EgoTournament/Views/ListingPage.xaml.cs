using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.Views;

public partial class ListingPage : ContentPage
{
    private readonly IAuthService _authService;

    public ListingPage(IAuthService authService)
    {
        InitializeComponent();
        this._authService = authService;
        GetProfileInfo();
    }

    private async void GetProfileInfo()
    {
        var stringCurrentUser = await SecureStorage.GetAsync(Globals.CURRENT_USER);
        var currentUser = !string.IsNullOrEmpty(stringCurrentUser) ? JsonConvert.DeserializeObject<FirebaseUserDto>(stringCurrentUser) : null;
        UserEmail.Text = currentUser?.Info?.Email;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _authService.GetCurrentAuthenticatedUserAsync() != null)
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
            await Toast.Make("You should Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }
}