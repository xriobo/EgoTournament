using EgoTournament.Services;
using EgoTournament.Services.Implementations;
using Firebase.Auth.Providers;
using Firebase.Auth;
using Microsoft.Extensions.Configuration;

namespace EgoTournament.Tests.Infrastructue
{
    public class MauiProgramFixture : IDisposable
    {
        private const string FirebaseApiKey = "AIzaSyBw0Iys7DlSwPMXmhRDOC7fR8uA45La1Lc";
        private const string FirebaseAuthDomain = "egotournament1.firebaseapp.com";

        public ServiceProvider ServiceProvider { get; private set; }

        public MauiProgramFixture()
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFirebaseService, FirebaseService>();
            services.AddSingleton(SecureStorage.Default);
            services.AddSingleton(services => new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = FirebaseApiKey,
                AuthDomain = FirebaseAuthDomain,
                Providers =
                [
                        new EmailProvider(),
                    new GoogleProvider(),
            ],
            }));
        }

        public void Dispose()
        {
        }
    }
}