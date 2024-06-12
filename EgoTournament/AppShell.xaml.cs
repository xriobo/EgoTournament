namespace EgoTournament;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        Routing.RegisterRoute(nameof(TournamentPage), typeof(TournamentPage));
        Routing.RegisterRoute(nameof(ManageListPage), typeof(ManageListPage));
        Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
        Routing.RegisterRoute(nameof(SearchSummonerPage), typeof(SearchSummonerPage));
        Routing.RegisterRoute(nameof(RulesPage), typeof(RulesPage));
        Routing.RegisterRoute(nameof(PromptPage), typeof(PromptPage));
    }
}
