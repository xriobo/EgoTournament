using CommunityToolkit.Maui.Alerts;
using EgoTournament.Adapters;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Paypal;
using EgoTournament.Services;
using EgoTournament.Services.Implementations;
using System.ComponentModel;

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
        /// The riot service.
        /// </summary>
        private readonly IRiotService _riotService;

        /// <summary>
        /// The payment service.
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// The cache user.
        /// </summary>
        private UserDto _cacheUser;

        /// <summary>
        /// The order identifier.
        /// </summary>
        private string _approvalLink;

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
        /// Gets the finish tournament command.
        /// </summary>
        /// <value>
        /// The finish tournament command.
        /// </value>
        public IRelayCommand FinishCommand { get; }

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
        /// Gets the results command.
        /// </summary>
        /// <value>
        /// The results command.
        /// </value>
        public IRelayCommand ResultsCommand { get; }

        /// <summary>
        /// Gets or sets the tournaments.
        /// </summary>
        /// <value>
        /// The tournaments.
        /// </value>
        private ObservableCollection<TournamentDto> _tournaments;

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<TournamentDto> Tournaments
        {
            get => _tournaments;
            set
            {
                _tournaments = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            _firebaseService = App.Services.GetService<IFirebaseService>();
            _cacheService = App.Services.GetService<ICacheService>();
            _riotService = App.Services.GetService<IRiotService>();
            _paymentService = App.Services.GetService<IPaymentService>();

            Tournaments = new ObservableCollection<TournamentDto>();

            DeleteCommand = new AsyncRelayCommand<TournamentDto>(DeleteTournament);
            UpdateCommand = new AsyncRelayCommand<TournamentDto>(UpdateTournament);
            TournamentSelectedCommand = new AsyncRelayCommand<TournamentDto>(OnTournamentSelected);
            AddCommand = new AsyncRelayCommand(AddTournament);
            FinishCommand = new AsyncRelayCommand<TournamentDto>(FinishTournament);
            ResultsCommand = new AsyncRelayCommand<TournamentDto>(GoToResults);
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
            var userAllTournaments = await _firebaseService.GetTournamentsBySummonerNameAsync(cacheUser.SummonerName);
            if (cacheUser.Tournaments != null)
            {
                cacheUser.Tournaments = cacheUser.Tournaments.Concat(userAllTournaments).GroupBy(x => x.Uid).Select(g => g.First()).ToList();
            }
            else
            {
                cacheUser.Tournaments = userAllTournaments;
            }

            if (cacheUser.Tournaments != null && cacheUser.Tournaments.Count() > 0)
            {
                await _cacheService.SetCurrentUserAsync(cacheUser);
                foreach (var tournament in cacheUser.Tournaments)
                {
                    Tournaments.Add(tournament);
                }
            }
        }

        private async Task AddTournament()
        {
            if (_cacheUser.Role < RoleType.Admin)
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_ACCOUNT_TITLE, Globals.UPGRADE_ACCOUNT_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(TournamentPage));
            }
        }

        private async Task DeleteTournament(TournamentDto tournament)
        {
            if (_cacheUser.Role < RoleType.Admin)
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_ACCOUNT_TITLE, Globals.UPGRADE_ACCOUNT_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
            else if (_cacheUser.Role < RoleType.Premium && tournament.Finished)
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_PREMIUM_TITLE, Globals.UPGRADE_PREMIUM_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
            else
            {
                if (tournament == null)
                    return;
                if (_cacheUser.TournamentUids.Contains(tournament.Uid))
                {
                    await Shell.Current.GoToAsync($"//{nameof(PromptPage)}", true, new Dictionary<string, object>
                    {
                        { nameof(MethodType), MethodType.Main },
                        { nameof(TournamentDto), tournament }
                    });
                }
                else
                {
                    await Toast.Make(Globals.NOT_OWNER_TITLE, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
        }

        private async Task UpdateTournament(TournamentDto tournament)
        {
            if (_cacheUser.Role < RoleType.Admin)
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_ACCOUNT_TITLE, Globals.UPGRADE_ACCOUNT_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
            else
            {
                if (tournament == null)
                    return;

                if (_cacheUser.TournamentUids.Contains(tournament.Uid))
                {
                    await Shell.Current.GoToAsync(nameof(TournamentPage), true, new Dictionary<string, object>
                    {
                        { nameof(TournamentDto), tournament }
                    });
                }
                else
                {
                    await Toast.Make(Globals.NOT_OWNER_TITLE, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
        }

        private async Task FinishTournament(TournamentDto tournament)
        {
            if (_cacheUser.Role < RoleType.Admin)
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_ACCOUNT_TITLE, Globals.UPGRADE_ACCOUNT_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
            else
            {
                string toastMessage = Globals.NOT_OWNER_TITLE;
                if (_cacheUser.TournamentUids.Contains(tournament.Uid))
                {
                    tournament.FinishDate = DateTime.Now;
                    tournament.Finished = true;
                    var tournamentFinished = tournament.ToTournamentResult();
                    var tuple = await _riotService.GetPuuidByParticipantsNameAndTagName(tournament.SummonerNames);
                    var summonersRiot = await _riotService.GetSummonersByPuuid(tuple.Item1);
                    var summonerWithRanks = await _riotService.SetParticipantRanks(summonersRiot);
                    tournamentFinished.ParticipantsResults = summonerWithRanks.ToParticipantsResult().ToList();
                    var tournamentResult = await _firebaseService.UpsertTournamentsHistoryAsync(tournamentFinished);
                    toastMessage = "Error finishing tournament.";
                    if (tournamentResult != null)
                    {
                        var tournamentUpdated = await _firebaseService.UpsertTournamentAsync(tournament: tournament, summonersToAssign: new List<string>(), summonersToUnassign: tournament.SummonerNames);
                        var currentUser = await _cacheService.GetCurrentUserAsync();
                        int index = currentUser.Tournaments.FindIndex(obj => obj.Uid == tournament.Uid);
                        currentUser.Tournaments[index] = tournamentUpdated;
                        await _cacheService.SetCurrentUserAsync(await _firebaseService.UpsertUserAsync(currentUser));
                        _cacheUser = currentUser;
                        Tournaments = new ObservableCollection<TournamentDto>(currentUser.Tournaments);
                        toastMessage = tournamentUpdated != null ? "Finished tournament." : toastMessage;
                    }
                }

                await Toast.Make(toastMessage, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }

        private async Task GoToResults(TournamentDto tournament)
        {
            if (_cacheUser.Role > RoleType.Admin)
            {
                var tournamentResult = await _firebaseService.GetTournamentsHistoryAsync(tournament.Uid);
                await Shell.Current.GoToAsync(nameof(TournamentResultPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentResultDto), tournamentResult }
                });
            }
            else
            {
                if (await Shell.Current.DisplayAlert(Globals.UPGRADE_PREMIUM_TITLE, Globals.UPGRADE_PREMIUM_MESSAGE, "YES", "NO"))
                {
                    await RedirectToPaymentService();
                }
            }
        }

        private async Task RedirectToPaymentService()
        {
            var payment = new PaymentDto() { Currency = Globals.CURRENCY };
            if (_cacheUser.Role < RoleType.Premium)
            {
                payment.Amount = "10.00";
            }

            if (_cacheUser.Role < RoleType.Admin)
            {
                payment.Amount = "5.00";
            }

            _approvalLink = await _paymentService.CreateOrderAsync(payment);

            if (!string.IsNullOrEmpty(_approvalLink))
            {
                await Browser.OpenAsync(_approvalLink, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}
