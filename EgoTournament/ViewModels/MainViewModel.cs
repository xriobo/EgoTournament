using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    /// <summary>
    /// Main View Model class.
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    /// <seealso cref="INotifyPropertyChanged" />
    public partial class MainViewModel : BaseViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// The firebase service.
        /// </summary>
        private readonly IFirebaseService _firebaseService;

        /// <summary>
        /// The cache service.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// The cache user.
        /// </summary>
        private UserDto _cacheUser;

        /// <summary>
        /// The visible by role.
        /// </summary>
        [ObservableProperty]
        public bool canManageTournaments = false;

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        /// <value>
        /// The delete command.
        /// </value>
        public IRelayCommand DeleteCommand { get; }

        /// <summary>
        /// Gets the update command.
        /// </summary>
        /// <value>
        /// The update command.
        /// </value>
        public IRelayCommand UpdateCommand { get; }

        /// <summary>
        /// Gets the tournament selected command.
        /// </summary>
        /// <value>
        /// The tournament selected command.
        /// </value>
        public IRelayCommand TournamentSelectedCommand { get; }

        /// <summary>
        /// Gets the add command.
        /// </summary>
        /// <value>
        /// The add command.
        /// </value>
        public IRelayCommand AddCommand { get; }

        /// <summary>
        /// Gets or sets the tournaments.
        /// </summary>
        /// <value>
        /// The tournaments.
        /// </value>
        public ObservableCollection<TournamentDto> Tournaments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
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

        public async Task OnNavigatedTo()
        {
            var userCredentials = await _cacheService.GetCurrentUserCredentialAsync();
            _cacheUser = await _cacheService.GetCurrentUserAsync();
            if (userCredentials == null)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }

            if (_cacheUser != null)
            {
                await LoadTournamentsAndSetVisibleByCacheUser(_cacheUser);
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("PROFILE", "Set up your profile to continue.", "OK");
                await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
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

        private async Task LoadTournamentsAndSetVisibleByCacheUser(UserDto cacheUser)
        {
            Tournaments.Clear();
            if (cacheUser.Role == RoleType.Basic)
            {
                var basicUserTournaments = await _firebaseService.GetTournamentsBySummonerNameAsync(cacheUser.SummonerName);
                cacheUser.Tournaments = basicUserTournaments;
            }
            else
            {
                CanManageTournaments = true;
            }

            if (cacheUser.Tournaments != null && cacheUser.Tournaments.Count() > 0)
            {
                foreach (var tournament in cacheUser.Tournaments)
                {
                    Tournaments.Add(tournament);
                }
            }
        }

        private async Task DeleteTournament(TournamentDto tournament)
        {
            if (_cacheUser.Role == RoleType.Basic)
            {
                await Shell.Current.DisplayAlert("Premium", "If u want create, modify and delete tournaments make your account premium.", "OK");
            }
            else
            {
                if (tournament == null)
                    return;

                await Shell.Current.GoToAsync(nameof(PromptPage), true, new Dictionary<string, object>
                {
                    { nameof(MethodType), MethodType.Main },
                    { nameof(TournamentDto), tournament }
                });
            }
        }

        private async Task UpdateTournament(TournamentDto tournament)
        {
            if (_cacheUser.Role == RoleType.Basic)
            {
                await Shell.Current.DisplayAlert("Premium", "If u want create, modify and delete tournaments make your account premium.", "OK");
            }
            else
            {
                if (tournament == null)
                    return;

                await Shell.Current.GoToAsync(nameof(TournamentPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), tournament }
                });
            }
        }

        private async Task AddTournament()
        {
            if (_cacheUser.Role == RoleType.Basic)
            {
                await Shell.Current.DisplayAlert("Premium", "If u want create, modify and delete tournaments make your account premium.", "OK");
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(TournamentPage));
            }
        }
    }
}
