using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public partial class TournamentViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public TournamentDto Tournament { get; set; }
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
            this._firebaseService = App.Services.GetService<IFirebaseService>();
            this._cacheService = App.Services.GetService<ICacheService>();
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

                    if (Tournament.Uid != Guid.Empty)
                    {
                        if (!AreSameTournaments(Tournament, tournament))
                        {
                            tournament.HasReward = Tournament.HasReward;
                            tournament.Rules = Tournament.Rules;
                            tournament.SummonerNames = Tournament.SummonerNames;
                            tournament.Name = Tournament.Name;
                        }
                    }
                    else
                    {
                        currentUser.Tournaments.Add(Tournament);
                        await Toast.Make($"Added newTournament Successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }


                    var userUpdated = await _firebaseService.PutUser(currentUser);
                    await _cacheService.SetCurrentUserAsync(userUpdated);
                    await Shell.Current.GoToAsync("..");
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
            var modalPage = new ListModalPage(Summoners, "Participants", true);
            modalPage.ValuesUpdated += (sender, updatedItems) =>
            {
                Summoners.Clear();
                foreach (var summonerName in updatedItems)
                {
                    Summoners.Add(summonerName);
                }
            };

            await Shell.Current.Navigation.PushAsync(modalPage, true);
        }

        private async Task OpenRulesListModal()
        {
            var modalPage = new ListModalPage(Rules, "Rules");
            modalPage.ValuesUpdated += (sender, updatedItems) =>
            {
                Rules.Clear();
                foreach (var rule in updatedItems)
                {
                    Rules.Add(rule);
                }
            };

            await Shell.Current.Navigation.PushAsync(modalPage, true);
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

        public void Initialize(TournamentDto tournament)
        {
            Tournament = tournament;
            //RulesText = string.Join(Environment.NewLine, Tournament.Rules);
            //SummonerNamesText = string.Join(Environment.NewLine, Tournament.SummonerNames);
        }
    }
}
