using EgoTournament.Models;
using Firebase.Auth;

namespace EgoTournament.Services
{
    public interface IFirebaseService
    {
        Task<UserCredential> SignInAsync(string email, string password);

        Task<UserCredential> SignUpAsync(string email, string password);

        Task DeleteUserAndUserCredentialAsync(string userUid);

        Task<UserDto> UpsertUserAsync(UserDto userDto);

        Task<UserDto> GetUserByUidAsync(string userUid);

        Task<TournamentDto> UpsertTournamentAsync(TournamentDto tournament, bool updateSummonerTournamentsRelation = true);

        Task<List<TournamentDto>> GetTournamentsByUidsAsync(List<Guid> tournamentUids);

        Task<List<TournamentDto>> GetTournamentsBySummonerNameAsync(string summonerName);
    }
}
