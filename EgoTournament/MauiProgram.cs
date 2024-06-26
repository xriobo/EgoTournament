﻿using EgoTournament.Services;
using EgoTournament.Services.Implementations;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Logging;

namespace EgoTournament;

public static class MauiProgram
{
    private const string FirebaseApiKey = "AIzaSyBw0Iys7DlSwPMXmhRDOC7fR8uA45La1Lc";
    private const string FirebaseAuthDomain = "egotournament1.firebaseapp.com";

    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("FontAwesome6FreeBrands.otf", "FontAwesomeBrands");
				fonts.AddFont("FontAwesome6FreeRegular.otf", "FontAwesomeRegular");
				fonts.AddFont("FontAwesome6FreeSolid.otf", "FontAwesomeSolid");
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Services
        builder.Services.AddTransient<ICacheService, CacheService>();
        builder.Services.AddTransient<IFirebaseService, FirebaseService>();
        builder.Services.AddTransient<IRiotService, RiotService>();
        builder.Services.AddTransient<IPaymentService, PaymentService>();

        // Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoadingPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SignUpPage>();
        builder.Services.AddTransient<TournamentPage>();
        builder.Services.AddTransient<PromptPage>();
        builder.Services.AddTransient<ManageListPage>();
        builder.Services.AddTransient<SchedulePage>();
        builder.Services.AddTransient<SearchSummonerPage>();
        builder.Services.AddTransient<RulesPage>();
        builder.Services.AddTransient<TournamentResultPage>(); 


        // ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<PromptViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<TournamentViewModel>();
        builder.Services.AddTransient<ScheduleViewModel>();
        builder.Services.AddTransient<SearchSummonerViewModel>();
        builder.Services.AddTransient<RulesViewModel>();
        builder.Services.AddTransient<TournamentResultViewModel>();

        // Authentication
        builder.Services.AddSingleton(SecureStorage.Default);
        builder.Services.AddSingleton(services => new FirebaseAuthClient(new FirebaseAuthConfig()
        {
            ApiKey = FirebaseApiKey,
            AuthDomain = FirebaseAuthDomain,
            Providers =
            [
                    new EmailProvider(),
                    new GoogleProvider(),
            ],
        }));

        return builder.Build();
    }
}