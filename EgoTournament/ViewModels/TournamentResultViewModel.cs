using EgoTournament.Models;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    public partial class TournamentResultViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        public ObservableCollection<ParticipantResultDto> Participants { get; set; }

        private readonly ICacheService _cacheService;

        [ObservableProperty]
        TournamentResultDto tournamentResult;

        public IAsyncRelayCommand BackCommand { get; }

        public TournamentResultViewModel()
        {
            _cacheService = App.Services.GetService<ICacheService>();

            Participants = new ObservableCollection<ParticipantResultDto>();
            BackCommand = new AsyncRelayCommand(GoToMainPage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.Any()) return;
            TournamentResult = query[nameof(TournamentResultDto)] as TournamentResultDto;
            if (TournamentResult != null)
            {
                if (TournamentResult.ParticipantsResults.Any())
                {
                    foreach (var participantResult in TournamentResult.ParticipantsResults)
                    {
                        Participants.Add(participantResult);
                    }
                }
            }
        }

        private async Task GoToMainPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}", true);
        }
    }
}