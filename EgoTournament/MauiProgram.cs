using Syncfusion.Maui.Core.Hosting;

namespace EgoTournament;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("FontAwesome6FreeBrands.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesome6FreeRegular.otf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesome6FreeSolid.otf", "FontAwesomeSolid");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<SampleDataService>();
        builder.Services.AddTransient<TournamentsDetailViewModel>();
        builder.Services.AddTransient<TournamentsDetailPage>();
        builder.Services.AddSingleton<TournamentsViewModel>();
        builder.Services.AddSingleton<TournamentsPage>();

        builder.ConfigureSyncfusionCore();

        return builder.Build();
    }
}
