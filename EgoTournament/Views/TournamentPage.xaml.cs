using EgoTournament.Models;

namespace EgoTournament.Views;

[QueryProperty("Tournament", "Tournament")]
public partial class TournamentPage : ContentPage
{
    public TournamentDto Tournament
    {
        set
        {
            var viewModel = (TournamentViewModel)BindingContext;
            viewModel.Initialize(value);
        }
    }

    public TournamentPage()
    {
        InitializeComponent();
    }
}