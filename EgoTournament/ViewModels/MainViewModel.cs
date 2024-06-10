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

        public IRelayCommand RefreshingCommand { get; }

        public IRelayCommand AddCommand { get; }

        /// <summary>
        /// The selected <see cref="TournamentDto"/>.
        /// </summary>
        private TournamentDto _selectedItem;

        /// <summary>
        /// The new item name.
        /// </summary>
        private string _newItemName;

        /// <summary>
        /// The is refreshing.
        /// </summary>
        private bool _isRefreshing;

        public MainViewModel()
        {
            this._firebaseService = App.Services.GetService<IFirebaseService>();
            this._cacheService = App.Services.GetService<ICacheService>();

            Tournaments = new ObservableCollection<TournamentDto>();

            DeleteCommand = new AsyncRelayCommand<TournamentDto>(DeleteTournament);
            UpdateCommand = new AsyncRelayCommand<TournamentDto>(UpdateTournament);
            TournamentSelectedCommand = new AsyncRelayCommand<TournamentDto>(OnTournamentSelected);
            RefreshingCommand = new AsyncRelayCommand(OnRefreshing);
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

            ////var navigationParameter = new Dictionary<string, object>
            ////    {
            ////        { "Tournament", tournament }
            ////    };

            ////await Shell.Current.GoToAsync(nameof(TournamentPage), navigationParameter);
            await Toast.Make("CARGAR DATOS DEL TORNEO RIOT API.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
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

        private async Task OnRefreshing()
        {
            IsRefreshing = true;

            try
            {
                await LoadTournaments();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public ObservableCollection<TournamentDto> Tournaments { get; set; }

        public string NewItemName
        {
            get => _newItemName;
            set
            {
                _newItemName = value;
                OnPropertyChanged();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
    }
}
