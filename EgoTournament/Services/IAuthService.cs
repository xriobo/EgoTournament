using EgoTournament.Models.Firebase;

namespace EgoTournament.Services
{
    public interface IAuthService
    {
        Task<FirebaseUserDto> GetCurrentAuthenticatedUserAsync();

        Task<bool> FirstIsAuthenticatedAsync();

        void Logout();
    }
}
