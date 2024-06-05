using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Models.Behaviors;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ICacheService _cacheService;

    private readonly IFirebaseService _firebaseService;

    private FirebaseUserDto userCredentials;

    private UserDto currentUser;

    public List<string> RoleValues { get; } = Enum.GetNames(typeof(RoleType)).ToList();

    public string SummonerName { get; set; }

    private const string ErrorSummonerNameValidation = "SummonerName must contain a hash symbol in the middle but no special characters. Lenght must be 5 or more.";

    public ProfilePage(ICacheService cacheService, IFirebaseService firebaseService)
    {
        _cacheService = cacheService;
        _firebaseService = firebaseService;
        InitializeComponent();
        BindingContext = this;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        userCredentials = await _cacheService.GetCurrentUserCredentialAsync();
        if (userCredentials != null)
        {
            currentUser = await _cacheService.GetCurrentUserAsync();
            LoadScreenData(currentUser);
            await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
        }
        else
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }

    private void LoadScreenData(UserDto user)
    {
        if (user != null)
        {
            currentUser = user;
            entrySummoner.Text = user.SummonerName;
            entrySummoner.IsEnabled = false;
            rolePicker.SelectedIndex = (int)user.Role;
            rolePicker.IsEnabled = false;
            saveBtn.IsVisible = false;
            lblenableSummonerName.IsVisible = true;
            enableSummonerName.IsVisible = true;
            enableSummonerName.IsChecked = false;
        }
        else
        {
            rolePicker.SelectedIndex = (int)default(RoleType);
            entrySummoner.Text = null;
            entrySummoner.IsEnabled = true;
            rolePicker.IsEnabled = true;
            saveBtn.IsVisible = true;
        }
    }

    private async void Logout_Clicked(object sender, EventArgs e)
    {
        _cacheService.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        await Toast.Make("Goodbye!", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!enableSummonerName.IsChecked)
            {
                var role = Enum.Parse<RoleType>(rolePicker.SelectedItem.ToString(), true);
                if (await Validations(entrySummoner.Behaviors.OfType<EntryValidationBehavior>().FirstOrDefault()))
                {
                    if (currentUser == null)
                    {
                        currentUser = new UserDto()
                        {
                            Role = role,
                            SummonerName = SummonerName,
                            Uid = userCredentials.Uid,
                            Email = userCredentials.Info.Email,
                        };

                        await _firebaseService.CreateUser(currentUser);
                        await _cacheService.SetCurrentUserAsync(currentUser);
                        LoadScreenData(currentUser);
                    }
                    else
                    {
                        await Toast.Make("Error: User already exists in the database..", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }
                }
            }
            else
            {
                currentUser.SummonerName = SummonerName;
                if (await DisplayAlert("WARNING", "Do you confirm that you want to modify the summonerName?", "YES", "NO")
                        && await _firebaseService.PutUser(currentUser) != null)
                {
                    await _cacheService.SetCurrentUserAsync(currentUser);
                    await Toast.Make("Updated successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
                else
                {
                    currentUser = await _cacheService.GetCurrentUserAsync();
                }

                LoadScreenData(currentUser);
            }
        }
        catch (Exception ex)
        {
            await Toast.Make("Failed to load profile. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
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

    private async void OnCheckedChange(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && await AppShell.Current.DisplayAlert("WARNING", "If the summoner name is modified, the assigned tournaments will change to the new name.", "Ok", "Cancel"))
        {
            entrySummoner.IsEnabled = true;
            saveBtn.IsVisible = true;
        }
        else
        {
            enableSummonerName.IsChecked = false;
            entrySummoner.IsEnabled = false;
            saveBtn.IsVisible = false;
        }
    }

    private void DeleteAccount_Clicked(object sender, EventArgs e)
    {
        ShowPrompt(sender, e);
    }

    private async void ShowPrompt(object sender, EventArgs e)
    {
        var promptPage = new PromptPage(_cacheService, _firebaseService);
        await Navigation.PushModalAsync(promptPage);
    }
}