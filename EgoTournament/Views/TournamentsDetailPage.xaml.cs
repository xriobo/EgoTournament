namespace EgoTournament.Views;

public partial class TournamentsDetailPage : ContentPage
{
	public TournamentsDetailPage(TournamentsDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
