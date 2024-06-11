using EgoTournament.Models;
using EgoTournament.Models.Riot;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    public partial class ScheduleViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        public IRelayCommand RefreshingCommand { get; }

        public IAsyncRelayCommand BackCommand { get; }

        public IAsyncRelayCommand ShowRulesCommand { get; }

        private readonly IRiotService _riotService;
        private readonly ICacheService _cacheService;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        public ObservableCollection<SummonerDto> summoners;

        [ObservableProperty]
        TournamentDto tournament;

        public static List<string> Participants;
        public static List<SummonerDto> SummonerDtos;

        public ScheduleViewModel()
        {
            _cacheService = App.Services.GetService<ICacheService>();
            _riotService = App.Services.GetService<IRiotService>();
            Participants = new List<string>();
            Summoners = new ObservableCollection<SummonerDto>();

            RefreshingCommand = new RelayCommand(Refreshing);
            BackCommand = new AsyncRelayCommand(BackToMain);
            ShowRulesCommand = new AsyncRelayCommand(ShowRules);
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
                            Participants.Add(summonerName);
                        }
                    }
                }
            }

            LoadSchedule();
        }

        public void LoadSchedule()
        {
            try
            {
                List<SummonerDto> summonerDtos = new List<SummonerDto>();
                var puuidDtos = _riotService.GetPuuidByParticipantsNameAndTagName(Participants);
                var summonersRiot = _riotService.GetSummonersByPuuid(puuidDtos);
                summonerDtos = _riotService.SetParticipantRanks(summonersRiot).ToList();
                summonerDtos = GetOrdererSummoners(summonerDtos);
                foreach (var summonerDto in summonerDtos)
                {
                    Summoners.Add(summonerDto);
                }
                Summoners = new ObservableCollection<SummonerDto>(summonerDtos);
                SummonerDtos = summonerDtos;
            }
            catch (Exception ex)
            {
                Summoners = new ObservableCollection<SummonerDto>(Mocks.GetSummonerErrorDto(ex.Message));
            }
        }

        private List<SummonerDto> GetOrdererSummoners(IEnumerable<SummonerDto> summonerDtos)
        {
            return summonerDtos.OrderByDescending(x => (int)x.RankSoloQ.TierType).ThenByDescending(x => (int)x.RankSoloQ.Division).ThenByDescending(x => x.RankSoloQ.LeaguePoints).ToList();
        }

        private void Refreshing()
        {
            IsRefreshing = true;

            try
            {
                LoadSchedule();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task BackToMain()
        {
            Summoners.Clear();
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

        private async Task ShowRules()
        {
            if (!Tournament.Rules.Any())
                return;

            await Shell.Current.GoToAsync(nameof(RulesPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), Tournament }
                });
        }
    }
}