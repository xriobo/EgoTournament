using EgoTournament.Models;

namespace EgoTournament.ViewModels
{
    public partial class ManageListViewModal : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        [ObservableProperty]
        TournamentDto tournament;

        public ObservableCollection<string> Items { get; }

        public ManageListViewModal()
        {
            Items = new ObservableCollection<string>();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Tournament = query[nameof(TournamentDto)] as TournamentDto;
            var typeOfList = query["type"] as string;
            if (Tournament != null && !string.IsNullOrEmpty(typeOfList))
            {
                if (Tournament.SummonerNames.Any())
                {
                    foreach (var summonerName in Tournament.SummonerNames)
                    {
                        if (!string.IsNullOrEmpty(summonerName))
                        {
                            Items.Add(summonerName);
                        }
                    }
                }
            }
        }

        private void OnRemoveItemButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as string;

            if (item != null && Items.Contains(item))
            {
                Items.Remove(item);
            }
        }
    }
}
