using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public partial class TournamentViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        TournamentDto tournament;

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand ManageSummonersCommand { get; }
        public IRelayCommand ManageRulesCommand { get; }

        public ObservableCollection<string> Rules { get; }

        public ObservableCollection<string> Summoners { get; }

        private readonly ICacheService _cacheService;

        private readonly IFirebaseService _firebaseService;

        private const string RulesCountText = "Number of Rules ";

        private const string ParticipantsCountText = "Number of Participants ";

        public TournamentViewModel()
        {
            _firebaseService = App.Services.GetService<IFirebaseService>();
            _cacheService = App.Services.GetService<ICacheService>();

            Rules = new ObservableCollection<string>();
            Summoners = new ObservableCollection<string>();

            Tournament = new TournamentDto();
            SaveCommand = new AsyncRelayCommand<TournamentDto>(SaveTournament);
            CancelCommand = new AsyncRelayCommand(CancelClicked);
            ManageRulesCommand = new AsyncRelayCommand(OpenRulesListModal);
            ManageSummonersCommand = new AsyncRelayCommand(OpenSummonersListModal);
        }

        private async Task SaveTournament(TournamentDto tournament)
        {
            try
            {
                if (!string.IsNullOrEmpty(tournament.Name) && tournament.SummonerNames.Any())
                {
                    var currentUser = await _cacheService.GetCurrentUserAsync();
                    var oldTournament = currentUser.Tournaments.FirstOrDefault(x => x.Uid == Tournament.Uid);

                    bool existOldTournament = oldTournament != null;
                    bool isNewTournament = tournament.Uid == Guid.Empty || !existOldTournament;
                    bool areSameTournament = existOldTournament && AreSameTournaments(tournament, oldTournament);
                    if (isNewTournament)
                    {
                        tournament.Uid = Guid.NewGuid();
                        currentUser.Tournaments.Add(Tournament);
                        await Toast.Make($"Added Tournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }
                    else if (!areSameTournament)
                    {
                        oldTournament.HasReward = Tournament.HasReward;
                        oldTournament.Rules = Tournament.Rules;
                        oldTournament.SummonerNames = Tournament.SummonerNames;
                        oldTournament.Name = Tournament.Name;
                        await Toast.Make($"Updated Tournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }

                    if (isNewTournament || !areSameTournament)
                    {
                        var userUpdated = await _firebaseService.PutUser(currentUser);
                        await _cacheService.SetCurrentUserAsync(userUpdated);
                    }
                    else
                    {
                        await Toast.Make($"There are no changes.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }

                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    await Toast.Make("Tournament name and a participant are required.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
            catch (FirebaseAuthHttpException ex)
            {
                var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
                await Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to create newTournament. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }

        private async Task OpenSummonersListModal()
        {
            if (Tournament == null) return;
            await Shell.Current.GoToAsync(nameof(ManageListPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), Tournament },
                    { nameof(ListType), ListType.Summoners.ToString() }
                });
        }

        private async Task OpenRulesListModal()
        {
            if (Tournament == null) return;
            await Shell.Current.GoToAsync(nameof(ManageListPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), Tournament },
                    { nameof(ListType), ListType.Rules.ToString() }
                });
        }

        private bool AreSameTournaments(TournamentDto tournamentToUpdate, TournamentDto tournamentDto)
        {
            if (tournamentToUpdate.HasReward == tournamentDto.HasReward
                    && tournamentToUpdate.Name.Equals(tournamentDto.Name)
                    && tournamentToUpdate.SummonerNames.SequenceEqual(tournamentDto.SummonerNames)
                    && tournamentToUpdate.Rules.SequenceEqual(tournamentDto.Rules))
            {
                return true;
            }

            return false;
        }

        private async Task CancelClicked()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.Any()) return;
            Tournament = query[nameof(TournamentDto)] as TournamentDto;
            if (Tournament != null)
            {
                if (Tournament.SummonerNames.Any())
                {
                    foreach (var summonerName in Tournament.SummonerNames)
                    {
                        if (!string.IsNullOrEmpty(summonerName))
                        {
                            Summoners.Add(summonerName);
                        }
                    }
                }

                if (Tournament.Rules.Any())
                {
                    foreach (var rule in Tournament.Rules)
                    {
                        if (!string.IsNullOrEmpty(rule))
                        {
                            Rules.Add(rule);
                        }
                    }
                }
            }
        }
    }
}
