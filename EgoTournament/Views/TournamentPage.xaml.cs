using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class TournamentPage : ContentPage
{
    private readonly ICacheService _cacheService;

    public ObservableCollection<string> SummonerEntries { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<string> RuleEntries { get; set; } = new ObservableCollection<string>();

    public string TournamentName { get; set; }

    public TournamentPage(ICacheService cacheService)
    {
        InitializeComponent();
        this._cacheService = cacheService;
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
        if (!string.IsNullOrEmpty(TournamentName) || (string.IsNullOrEmpty(TournamentName) && await DisplayAlert("Name is Empty.", "Do you want a random tournament name?", "OK", "Cancel")))
        {
            TournamentName = TournamentName ?? Guid.NewGuid().ToString();
            bool hasReward = HasRewardCheckBox.IsChecked;
            TournamentDto tournament = new TournamentDto(TournamentName, RuleEntries.ToList(), SummonerEntries.ToList(), hasReward);
            await DisplayAlert("Torneo Creado", $"Nombre: {tournament.Name}\nReglas: {string.Join(", ", tournament.Rules)}\nInvocadores: {string.Join(", ", tournament.SummonerNames)}\nRecompensa: {tournament.HasReward}", "OK");
        }
    }

    private async void OnOpenSummonerModalButtonClicked(object sender, EventArgs e)
    {
        var modalPage = new ListModalPage(SummonerEntries, "Participants", true);
        modalPage.ValuesUpdated += (sender, updatedItems) =>
        {
            SummonerEntries = updatedItems;
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
            BindingContext = this;
        };

        await Navigation.PushModalAsync(modalPage, true);
    }
}