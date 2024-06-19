namespace EgoTournament.Views;

public partial class TournamentResultPage : ContentPage
{
	public TournamentResultPage(TournamentResultViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }

    private void Image_SizeChanged(object sender, EventArgs e)
    {
        var image = (Image)sender;
        //image.IsAnimationPlaying = false;
        image.IsAnimationPlaying = true;
    }
}