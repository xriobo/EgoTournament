using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.Services.Implementations
{
    public class CacheService : ICacheService
    {
        public async Task<FirebaseUserDto> GetCurrentUserCredentialAsync()
        {
            var jsonCurrentUserCredential = await SecureStorage.GetAsync(Globals.CURRENT_USER_CREDENTIAL);
            return !string.IsNullOrEmpty(jsonCurrentUserCredential) ? JsonConvert.DeserializeObject<FirebaseUserDto>(jsonCurrentUserCredential) : null;
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var jsonCurrentUser = await SecureStorage.GetAsync(Globals.CURRENT_USER);
            return !string.IsNullOrEmpty(jsonCurrentUser) ? JsonConvert.DeserializeObject<UserDto>(jsonCurrentUser) : null;
        }

        public async Task SetCurrentUserCredentialAsync(User user)
        {
            await SecureStorage.SetAsync(Globals.CURRENT_USER_CREDENTIAL, JsonConvert.SerializeObject(user));
        }

        public async Task SetCurrentUserAsync(UserDto user)
        {
            await SecureStorage.SetAsync(Globals.CURRENT_USER, JsonConvert.SerializeObject(user));
        }

        public void Logout()
        {
            SecureStorage.RemoveAll();
        }

        public async Task<bool> FirstIsAuthenticatedAsync()
        {
            await Task.Delay(500);
            return !string.IsNullOrEmpty(await SecureStorage.GetAsync(Globals.CURRENT_USER_CREDENTIAL));
        }
    }
}