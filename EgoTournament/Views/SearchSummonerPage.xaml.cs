namespace EgoTournament.Views;

public partial class SearchSummonerPage : ContentPage
{
    public SearchSummonerPage(SearchSummonerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext  = viewModel;
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
}