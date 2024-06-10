namespace EgoTournament.Views;

public partial class TournamentPage : ContentPage
{
    public TournamentPage(TournamentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}