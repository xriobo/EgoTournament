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

        Task<TournamentDto> UpsertTournamentAsync(TournamentDto tournament, List<string> summonersToAssign, List<string> summonersToUnassign);

        Task<List<TournamentDto>> GetTournamentsByUidsAsync(List<Guid> tournamentUids);

        Task<List<TournamentDto>> GetTournamentsBySummonerNameAsync(string summonerName);

        Task<bool> DeleteTournamentAsync(Guid tournamentUid);

        Task<TournamentResultDto> UpsertTournamentsHistoryAsync(TournamentResultDto tournamentResult);

        Task<bool> DeleteTournamentsHistoryAsync(Guid tournamentResultUid);

        Task<TournamentResultDto> GetTournamentsHistoryAsync(Guid tournamentResultUid);
    }
}
