namespace EgoTournament.Views;

public partial class TournamentsPage : ContentPage
{
	TournamentsViewModel ViewModel;

	public TournamentsPage(TournamentsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = ViewModel = viewModel;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		await ViewModel.LoadDataAsync();
	}
}
