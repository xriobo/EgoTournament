using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;

namespace EgoTournament.Views;

public partial class ListModalPage : ContentPage
{
    public ObservableCollection<string> Items { get; set; }

    public event EventHandler<ObservableCollection<string>> ValuesUpdated;

    public string ItemValue { get; set; }

    private bool _isSummonerList;


    public ListModalPage(ObservableCollection<string> items, string title, bool hasSummonerNameValidator = false)
    {
        Items = new ObservableCollection<string>(items);
        Title = title;
        InitializeComponent();
        _isSummonerList = hasSummonerNameValidator;
        if (!_isSummonerList) { ValueEntry.Behaviors.Clear(); }
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
        ValuesUpdated?.Invoke(this, Items);

        await Shell.Current.Navigation.PopModalAsync(true);
    }

    private void TextChanged_Event(object sender, TextChangedEventArgs e)
    {
        if (validationMessage.IsVisible)
        {
            validationMessage.IsVisible = false;
        }
    }
}