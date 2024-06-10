namespace EgoTournament.Views;

public partial class TournamentPage : ContentPage
{
    public TournamentPage(TournamentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}