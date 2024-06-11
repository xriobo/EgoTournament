using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using Firebase.Auth;

namespace EgoTournament.Services
{
    public interface ICacheService
    {
        Task<FirebaseUserCredentialDto> GetCurrentUserCredentialAsync();

        Task<UserDto> GetCurrentUserAsync();

        Task SetCurrentUserAsync(UserDto user);

        Task SetCurrentUserCredentialAsync(User user);

        Task<bool> FirstIsAuthenticatedAsync();

        void Logout();
    }
}
