using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class LoadingPage : ContentPage
{
    private readonly IAuthService _authService;

    public LoadingPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _authService.FirstIsAuthenticatedAsync())
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}