using EgoTournament.Services;
using EgoTournament.Services.Implementations;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using EgoTournament.Models;

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

        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<IFirebaseService, FirebaseService>();
        builder.Services.AddTransient<ListingPage>();
        builder.Services.AddTransient<LoadingPage>();
        //builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<SignUpPage>();

        builder.Services.AddSingleton(SecureStorage.Default);
        builder.Services.AddSingleton<IUserRepository, SecureStorageUserRepository>();
        builder.Services.AddSingleton(services => new FirebaseAuthClient(new FirebaseAuthConfig()
        {
            ApiKey = FirebaseApiKey,
            AuthDomain = FirebaseAuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                    new EmailProvider(),
                    new GoogleProvider(),
            },
            UserRepository = services.GetRequiredService<IUserRepository>()
        }));
        builder.Services.AddSingleton<CurrentUserStore>();

        return builder.Build();
    }
}