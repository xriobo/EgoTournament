using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Enums;

namespace EgoTournament.Views;

public partial class ManageListPage : ContentPage, IQueryAttributable
{
    private TournamentDto _tournament;

    private string _requestListType;

    public ObservableCollection<string> Items { get; set; }

    public string ItemValue { get; set; }

    private bool _isSummonerList;

    public ManageListPage()
    {
        Items = new ObservableCollection<string>();
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnAddItemButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ItemValue))
        {
            await Toast.Make("Insert a value.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        else if (Items.Any(x => x.Equals(ItemValue, StringComparison.InvariantCultureIgnoreCase)))
        {
            await Toast.Make("Duplicate value.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        else
        {
            if (_isSummonerList && !Validations.SummonerName(ItemValue))
            {
                validationMessage.Text = Globals.SUMMONERNAME_VALIDATION_ERROR_MESSAGE;
                validationMessage.IsVisible = true;
                await Toast.Make($"SummonerName invalid.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            else
            {
                Items.Add(_isSummonerList ? ItemValue.ToUpperInvariant() : ItemValue);
                ValueEntry.Text = string.Empty;
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

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (_requestListType.Equals(ListType.Summoners.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            _tournament.SummonerNames = Items.ToList();
        }
        else if (_requestListType.Equals(ListType.Rules.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            _tournament.Rules = Items.ToList();
        }

        await Shell.Current.GoToAsync(nameof(TournamentPage), true, new Dictionary<string, object>
                {
                    { nameof(TournamentDto), _tournament }
                });
    }

    private void TextChanged_Event(object sender, TextChangedEventArgs e)
    {
        if (validationMessage.IsVisible)
        {
            validationMessage.IsVisible = false;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _tournament = query[nameof(TournamentDto)] as TournamentDto;
        _requestListType = query[nameof(ListType)] as string;
        if (_tournament != null)
        {
            _isSummonerList = _requestListType?.Equals(ListType.Summoners.ToString(), StringComparison.InvariantCultureIgnoreCase) ?? false;
            if (_isSummonerList)
            {
                Title = ListType.Summoners.ToString();
                if (_tournament.SummonerNames.Any())
                {
                    foreach (var summonerName in _tournament.SummonerNames)
                    {
                        if (!string.IsNullOrEmpty(summonerName))
                        {
                            Items.Add(summonerName);
                        }
                    }
                }
            }
            else
            {
                Title = ListType.Rules.ToString();
                ValueEntry.Behaviors.Clear();
                if (_tournament.Rules.Any())
                {
                    foreach (var rule in _tournament.Rules)
                    {
                        if (!string.IsNullOrEmpty(rule))
                        {
                            Items.Add(rule);
                        }
                    }
                }
            }
        }
    }
}