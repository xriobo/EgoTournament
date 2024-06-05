using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.Views;

public partial class PromptPage : ContentPage
{
    private readonly IFirebaseService _firebaseService;
    private readonly ICacheService _cacheService;

    public PromptPage(ICacheService cacheService, IFirebaseService firebaseService)
    {
        _cacheService = cacheService;
        _firebaseService = firebaseService;
        InitializeComponent();
    }

    private async void OnAcceptClicked(object sender, EventArgs e)
    {
        try
        {
            var userCredential = await _cacheService.GetCurrentUserCredentialAsync();
            var currentUser = await _firebaseService.SignIn(userCredential.Info.Email, PromptEntry.Text);
            await _firebaseService.DeleteUserAndUserCredential(currentUser.User.Uid);
            await currentUser.User.DeleteAsync();
            await DisplayAlert("Removed", "The account has been successfully deleted.", "OK");
            _cacheService.Logout();
            await Navigation.PopModalAsync();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        catch (FirebaseAuthHttpException ex)
        {
            await Toast.Make(JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData).Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Failed to load profile. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }

        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
    }
}