using EgoTournament.Models.Views;

namespace EgoTournament.Views;

public partial class SearchSummonerPage : ContentPage
{
    SearchSummonerViewModel ViewModel;
    public SearchSummonerPage(SearchSummonerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext  = ViewModel = viewModel;
    }

    private void Image_SizeChanged(object sender, EventArgs e)
    {
        var image = (Image)sender;
        image.IsAnimationPlaying = false;
        image.IsAnimationPlaying = true;
    }

    private void summonerNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = (SearchSummonerViewModel)BindingContext;
        viewModel.SummonerNameEntryTextChangedCommand.Execute(e.NewTextValue);
    }

    /// <summary>
    /// When overridden in a derived class, allows application developers to customize behavior immediately prior to the page becoming visible.
    /// </summary>
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.SummonerNameEntryText = null;
        ViewModel.SummonerView = null;
        ViewModel.MatchesViewModel = new ObservableCollection<MatchView>();
    }
}