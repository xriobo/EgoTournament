using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Linq;

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

        private async Task SaveTournament(TournamentDto tournament)
        {
            try
            {
                if (!string.IsNullOrEmpty(tournament.Name) && tournament.SummonerNames.Any())
                {
                    var currentUser = await _cacheService.GetCurrentUserAsync();
                    var oldTournament = currentUser.Tournaments?.FirstOrDefault(x => x.Uid == Tournament.Uid);
                    bool isNewTournament = tournament.Uid == Guid.Empty || oldTournament == null;
                    bool arentSameTournament = oldTournament != null && AreSameTournaments(tournament, oldTournament);
                    if (isNewTournament)
                    {
                        await ManageNewTournament(tournament, currentUser);
                        await _cacheService.SetCurrentUserAsync(currentUser);
                        await Toast.Make($"Added Tournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }
                    else if (!arentSameTournament)
                    {
                        await ManageUpdateTournament(oldTournament, tournament, currentUser);
                        await _cacheService.SetCurrentUserAsync(currentUser);
                        await Toast.Make($"Updated Tournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }

                    if (!isNewTournament && arentSameTournament)
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

        private async Task ManageNewTournament(TournamentDto tournament, UserDto currentUser)
        {
            tournament.Uid = Guid.NewGuid();
            tournament.OwnerId = currentUser.Uid;
            if (currentUser.TournamentUids == null)
            {
                currentUser.TournamentUids = new List<string>();
            }

            currentUser.TournamentUids.Add(tournament.Uid.ToString());
            if (currentUser.Tournaments == null)
            {
                currentUser.Tournaments = new List<TournamentDto>();
            }

            currentUser.Tournaments.Add(tournament);
            var userUpdated = await _firebaseService.UpsertUserAsync(currentUser);
            await _firebaseService.UpsertTournamentAsync(tournament: tournament, summonerNamesToAdd: tournament.SummonerNames, new List<string>());
        }

        private async Task ManageUpdateTournament(TournamentDto oldTournament, TournamentDto tournamentToUpdate, UserDto currentUser)
        {
            var oldTournamentUpdated = oldTournament;
            var summonersToAdd = tournamentToUpdate.SummonerNames.Except(oldTournament.SummonerNames).ToList();
            var summonersToDelete = oldTournament.SummonerNames.Except(tournamentToUpdate.SummonerNames).ToList();
            oldTournamentUpdated.HasReward = tournamentToUpdate.HasReward;
            oldTournamentUpdated.Rules = tournamentToUpdate.Rules;
            oldTournamentUpdated.SummonerNames = tournamentToUpdate.SummonerNames;
            oldTournamentUpdated.Name = tournamentToUpdate.Name;

            var tournamentUpdated = await _firebaseService.UpsertTournamentAsync(tournament: oldTournamentUpdated, summonerNamesToAdd: summonersToAdd, summonerNamesToDelete: summonersToDelete);
        }
    }
}
