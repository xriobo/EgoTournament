using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.Views;

public partial class TournamentPage : ContentPage
{
    private readonly ICacheService _cacheService;

    private readonly IFirebaseService _firebaseService;

    private const string RulesCountText = "Number of Rules ";

    private const string ParticipantsCountText = "Number of Participants ";

    public ObservableCollection<string> SummonerEntries { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<string> RuleEntries { get; set; } = new ObservableCollection<string>();

    public string TournamentName { get; set; }

    public TournamentPage(ICacheService cacheService, IFirebaseService firebaseService)
    {
        InitializeComponent();
        this._cacheService = cacheService;
        this._firebaseService = firebaseService;
        summonerEntriesCountLabel.Text = ParticipantsCountText + SummonerEntries.Count();
        ruleEntriesCountLabel.Text = RulesCountText + RuleEntries.Count();
        BindingContext = this;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _cacheService.GetCurrentUserCredentialAsync() != null)
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(TournamentPage)}");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }

    private async void OnCreateTournamentClicked(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(TournamentName) || (string.IsNullOrEmpty(TournamentName) && await Shell.Current.DisplayAlert("Name is Empty.", "Do you want a random tournament name?", "OK", "Cancel")))
            {
                TournamentName = TournamentName ?? Guid.NewGuid().ToString();
                var hasReward = HasRewardCheckBox.IsChecked;
                var tournament = new TournamentDto(TournamentName, RuleEntries.ToList(), SummonerEntries.ToList(), hasReward);
                var userWithNewTournament = await _cacheService.GetCurrentUserAsync();
                userWithNewTournament.Tournaments.Add(tournament);
                var userUpdated = await _firebaseService.PutUser(userWithNewTournament);
                await _cacheService.SetCurrentUserAsync(userUpdated);
                ClearInputScreen();
                await Toast.Make($"Added tournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }
        catch (FirebaseAuthHttpException ex)
        {
            var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
            await Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Failed to create tournament. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }

    private async void OnOpenSummonerModalButtonClicked(object sender, EventArgs e)
    {
        var modalPage = new ListModalPage(SummonerEntries, "Participants", true);
        modalPage.ValuesUpdated += (sender, updatedItems) =>
        {
            SummonerEntries = updatedItems;
            summonerEntriesCountLabel.Text = ParticipantsCountText + SummonerEntries.Count();
            BindingContext = this;
        };

        await Navigation.PushModalAsync(modalPage, true);
    }

    private async void OnOpenRulesModalButtonClicked(object sender, EventArgs e)
    {
        var modalPage = new ListModalPage(RuleEntries, "Rules");
        modalPage.ValuesUpdated += (sender, updatedItems) =>
        {
            RuleEntries = updatedItems;
            ruleEntriesCountLabel.Text = RulesCountText + RuleEntries.Count();
            BindingContext = this;
        };

        await Navigation.PushModalAsync(modalPage, true);
    }

    private void ClearInputScreen()
    {
        SummonerEntries.Clear();
        RuleEntries.Clear();
        TournamentName = string.Empty;
        HasRewardCheckBox.IsChecked = false;
    }
}