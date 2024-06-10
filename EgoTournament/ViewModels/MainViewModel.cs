using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Services;
using System.Windows.Input;

namespace EgoTournament.ViewModels
{
    public partial class MainViewModel : BindableObject
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ICacheService _cacheService;

        public IRelayCommand DeleteCommand { get; }

        public IRelayCommand UpdateCommand { get; }

        public IRelayCommand TournamentSelectedCommand { get; }

        public IRelayCommand AddCommand { get; }

        public MainViewModel()
        {
            _firebaseService = App.Services.GetService<IFirebaseService>();
            _cacheService = App.Services.GetService<ICacheService>();

            Tournaments = new ObservableCollection<TournamentDto>();

            DeleteCommand = new AsyncRelayCommand<TournamentDto>(DeleteTournament);
            UpdateCommand = new AsyncRelayCommand<TournamentDto>(UpdateTournament);
            TournamentSelectedCommand = new AsyncRelayCommand<TournamentDto>(OnTournamentSelected);
            AddCommand = new AsyncRelayCommand(AddTournament);
        }

        public async Task OnNavigatedToAsync()
        {
            if (await _cacheService.GetCurrentUserCredentialAsync() != null)
            {
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                Tournaments.Clear();
                await LoadTournaments();
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }

        public async Task OnTournamentSelected(TournamentDto tournament)
        {
            if (tournament == null)
                return;

            await Shell.Current.GoToAsync(nameof(SchedulePage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), tournament }
                });
        }

        private async Task LoadTournaments()
        {
            var cacheUser = await _cacheService.GetCurrentUserAsync();
            if (cacheUser?.Tournaments != null)
            {
                foreach (var tournament in cacheUser.Tournaments)
                {
                    Tournaments.Add(tournament);
                }
            }
        }

        private async Task DeleteTournament(TournamentDto tournament)
        {
            if (tournament == null)
                return;

            if (Tournaments.Contains(tournament))
            {
                Tournaments.Remove(tournament);
            }

            var navigation = App.Current.MainPage.Navigation;
            await navigation.PushModalAsync(new PromptPage(_cacheService, _firebaseService, MethodType.Main, Tournaments.ToList()));
        }

        private async Task UpdateTournament(TournamentDto tournament)
        {
            if (tournament == null)
                return;

            await Shell.Current.GoToAsync(nameof(TournamentPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), tournament }
                });
        }

        private async Task AddTournament()
        {
            await Shell.Current.GoToAsync(nameof(TournamentPage));
        }

        public ObservableCollection<TournamentDto> Tournaments { get; set; }
    }
}
