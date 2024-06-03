using EgoTournament.Common;
using EgoTournament.Models.Firebase;
using Newtonsoft.Json;

namespace EgoTournament.Services.Implementations
{
    public class AuthService : IAuthService
    {
        public async Task<FirebaseUserDto> GetCurrentAuthenticatedUserAsync()
        {
            var stringCurrentUser = await SecureStorage.GetAsync(Globals.CURRENT_USER);
            return !string.IsNullOrEmpty(stringCurrentUser) ? JsonConvert.DeserializeObject<FirebaseUserDto>(stringCurrentUser) : null; ;
        }

        public void Logout()
        {
            SecureStorage.RemoveAll();
        }

        public async Task<bool> FirstIsAuthenticatedAsync()
        {
            await Task.Delay(500);
            return !string.IsNullOrEmpty(await SecureStorage.GetAsync(Globals.CURRENT_USER));
        }
    }
}