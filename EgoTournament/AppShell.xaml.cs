namespace EgoTournament;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(TournamentsDetailPage), typeof(TournamentsDetailPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
    }
}
