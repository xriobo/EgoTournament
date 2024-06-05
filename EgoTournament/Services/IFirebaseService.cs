using EgoTournament.Models;
using Firebase.Auth;

namespace EgoTournament.Services
{
    public interface IFirebaseService
    {
        Task<UserCredential> SignIn(string email, string password);

        Task<UserCredential> SignUp(string email, string password);

        Task<UserDto> GetUserByUid(string userUid);

        Task<UserDto> PutUser(UserDto userDto);

        Task DeleteUserAndUserCredential(string userUid);

        Task CreateUser(UserDto user);

        Task CreateTournament(TournamentDto tournament);
    }
}
