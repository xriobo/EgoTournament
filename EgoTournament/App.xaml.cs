using EgoTournament.Services;

namespace EgoTournament;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    private readonly ICacheService _cacheService;

    public App(IServiceProvider serviceProvider)
    {
		InitializeComponent();

        Services = serviceProvider;
        MainPage = new AppShell();
        _cacheService = App.Services.GetService<ICacheService>();
    }

    protected override async void OnSleep()
    {
        Thread.Sleep(5000);
        _cacheService.Logout();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}
