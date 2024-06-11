using EgoTournament.Models;

namespace EgoTournament.ViewModels
{
    public partial class RulesViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        ObservableCollection<string> rules;

        private TournamentDto Tournament;

        public IAsyncRelayCommand BackCommand { get; }

        public RulesViewModel()
        {
            rules = new ObservableCollection<string>();
            BackCommand = new AsyncRelayCommand(Back);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.Any()) return;
            Tournament = query[nameof(TournamentDto)] as TournamentDto;
            if (Tournament != null && Tournament.Rules.Any())
            {
                foreach (var rule in Tournament.Rules)
                {
                    Rules.Add(rule);
                }
            }
        }

        private async Task Back()
        {
            await Shell.Current.GoToAsync(nameof(SchedulePage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), Tournament }
                });
        }
    }
}