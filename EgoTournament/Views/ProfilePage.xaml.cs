using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Behaviors;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using EgoTournament.Services.Implementations;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EgoTournament.Views;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthService _authService;

    private readonly IFirebaseService _firebaseService;

    public List<string> RoleValues { get; } = Enum.GetNames(typeof(RoleType)).ToList();

    public string SummonerName { get; set; }

    private const string ErrorSummonerNameValidation = "SummonerName must contain a hash symbol in the middle but no special characters. Lenght must be 5 or more.";
    public ProfilePage(IAuthService authService, IFirebaseService firebaseService)
    {
        _authService = authService;
        _firebaseService = firebaseService;
        InitializeComponent();
        rolePicker.SelectedIndex = (int)default(RoleType);
        BindingContext = this;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var currentAuthenticatedUser = await _authService.GetCurrentAuthenticatedUserAsync();
        if (currentAuthenticatedUser != null)
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
            var jsonUser = await _firebaseService.GetUserByUid(currentAuthenticatedUser.Uid);
            if (jsonUser != null)
            {
                var bbddUser = JsonConvert.DeserializeObject<Dictionary<string, UserDto>>(jsonUser);
                var user = bbddUser.Values.FirstOrDefault();
                Console.WriteLine(user);
            }
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You should Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        _authService.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        await Toast.Make("Goodbye!", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        try
        {
            var role = Enum.Parse<RoleType>(rolePicker.SelectedItem.ToString(), true);
            var currentUser = JsonConvert.DeserializeObject<FirebaseUserDto>(await SecureStorage.GetAsync(Globals.CURRENT_USER));
            if (await Validations(entrySummoner.Behaviors.OfType<EntryValidationBehavior>().FirstOrDefault()))
            {
                if (!await _firebaseService.UserExists(currentUser.Uid))
                {
                    await _firebaseService.CreateUser(new Models.UserDto()
                    {
                        Role = role,
                        SummonerName = SummonerName,
                        Uid = currentUser.Uid,
                        Email = currentUser.Info.Email,
                    });
                }
                else
                {
                    await Toast.Make($"Error: User already exists in the database..", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        }
    }

    private async Task<bool> Validations(EntryValidationBehavior entryValidationBehavior)
    {
        bool isValid = false;
        if (entryValidationBehavior == null || !entryValidationBehavior.IsValid)
        {
            validationMessage.Text = ErrorSummonerNameValidation;
            validationMessage.IsVisible = true;
            await Toast.Make($"SummonerName invalid: {SummonerName}", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        else
        {
            isValid = true;
        }

        return isValid;
    }

    private void TextChanged_Event(object sender, TextChangedEventArgs e)
    {
        if (validationMessage.IsVisible)
        {
            validationMessage.IsVisible = false;
        }
    }
}