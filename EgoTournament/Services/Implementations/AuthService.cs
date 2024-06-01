namespace EgoTournament.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private const string AuthStateKey = "AuthState";
        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(2000);

            var authState = Preferences.Default.Get(AuthStateKey, false);

            return authState;
        }
        public void Login()
        {
            Preferences.Default.Set(AuthStateKey, true);
        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}