using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Services;
using System.Windows.Input;

namespace EgoTournament.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ICacheService _cacheService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        public ObservableCollection<TournamentDto> tournaments;

        public ICommand DeleteCommand { get; }

        public MainViewModel(ICacheService cacheService, IFirebaseService firebaseService, INavigationService navigationService)
        {
            this._firebaseService = firebaseService;
            this._cacheService = cacheService;
            this._navigationService = navigationService;
            DeleteCommand = new Command<TournamentDto>(DeleteTournament);
        }

        public async Task<ObservableCollection<TournamentDto>> LoadData()
        {
            var cacheTournaments = new List<TournamentDto>();
            var cacheUser = await _cacheService.GetCurrentUserAsync();
            if (cacheUser != null)
            {
                cacheTournaments = cacheUser.Tournaments;
            }
            return new ObservableCollection<TournamentDto>(cacheTournaments);
        }

        private async void DeleteTournament(TournamentDto tournament)
        {
            if (Tournaments.Contains(tournament))
            {
                Tournaments.Remove(tournament);
            }

            await _navigationService.PushModalAsync(new PromptPage(_cacheService, _firebaseService, _navigationService, MethodType.Main, Tournaments.ToList()));
        }

        [RelayCommand]
        private void OnRefreshing()
        {
            IsRefreshing = true;

            try
            {
                Tournaments = LoadData().Result;
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public async Task OnNavigatedToAsync()
        {
            if (await _cacheService.GetCurrentUserCredentialAsync() != null)
            {
                Tournaments = await LoadData();
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }
    }
}
