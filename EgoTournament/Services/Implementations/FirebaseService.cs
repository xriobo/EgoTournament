using EgoTournament.Models;
using Firebase.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace EgoTournament.Services.Implementations
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly HttpClient _httpClient;
        private const string DatabaseConnection = "https://egotournament1-default-rtdb.europe-west1.firebasedatabase.app";
        private const string UsersNode = "/users/";
        private const string TournamentsNode = "/tournaments/";
        private const string SummonerTournamentsNode = "/summonerTournaments/";
        private const string TournamentsHistory = "/tournamentsHistory/";
        private const string NullResponse = "null";

        public FirebaseService(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
            _httpClient = new HttpClient();
        }

        public async Task<UserCredential> SignInAsync(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task<UserCredential> SignUpAsync(string email, string password)
        {
            return await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        public async Task DeleteUserAndUserCredentialAsync(string userUid)
        {
            var response = await _httpClient.DeleteAsync($"{DatabaseConnection}/users/{userUid}.json");
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDto> UpsertUserAsync(UserDto userDto)
        {
            UserDto userUpdated = null;
            if (userDto.Tournaments != null && userDto.Tournaments.Count() > 0)
            {
                userDto.Tournaments = userDto.Tournaments.Where(x => userDto.TournamentUids.Contains(x.Uid)).ToList();
            }
            
            var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{UsersNode}{userDto.Uid}.json", userDto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                userUpdated = JsonConvert.DeserializeObject<UserDto>(json);
            }

            return userUpdated;
        }

        public async Task<UserDto> GetUserByUidAsync(string userUid)
        {
            UserDto userDto = null;
            var response = await _httpClient.GetAsync($"{DatabaseConnection}{UsersNode}{userUid}.json");
            if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != NullResponse)
            {
                userDto = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
            }

            return userDto;
        }

        public async Task<TournamentDto> UpsertTournamentAsync(TournamentDto tournament, List<string> summonersToAssign, List<string> summonersToUnassign)
        {
            TournamentDto tournamentUpdated = null;
            var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{TournamentsNode}{tournament.Uid}.json", tournament);

            if (response.IsSuccessStatusCode)
            {
                tournamentUpdated = JsonConvert.DeserializeObject<TournamentDto>(await response.Content.ReadAsStringAsync());
            }

            if (summonersToAssign.Count() > 0)
            {
                await UpsertSummonerTournamentsAsync(tournament.Uid, summonersToAssign);
            }

            if (summonersToUnassign.Count() > 0)
            {
                await DeleteTournamentByUidAndNullableListOfSummonerNamesInSummonerTournaments(tournament.Uid, summonersToUnassign);
            }

            return tournamentUpdated;
        }

        public async Task<List<TournamentDto>> GetTournamentsByUidsAsync(List<Guid> tournamentUids)
        {
            var tournaments = new ConcurrentBag<TournamentDto>();

            await Task.Run(() =>
            {
                Parallel.ForEach(tournamentUids, async uid =>
                {
                    var tournament = await GetTournamentByUidAsync(uid);
                    if (tournament != null)
                    {
                        tournaments.Add(tournament);
                    }
                });
            });

            return tournaments.ToList();
        }

        public async Task<List<TournamentDto>> GetTournamentsBySummonerNameAsync(string summonerName)
        {
            var httpClient = new HttpClient();
            var tournaments = new List<TournamentDto>();
            var summonerNameDatabase = summonerName.Replace("#", "*");
            var summonerTournamentsResponse = await httpClient.GetStringAsync($"{DatabaseConnection}{SummonerTournamentsNode}{summonerNameDatabase}.json");

            if (summonerTournamentsResponse.Equals(NullResponse, StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<TournamentDto>();
            }

            var tournamentIds = JsonConvert.DeserializeObject<Dictionary<string, bool>>(summonerTournamentsResponse);


            var tasks = tournamentIds.Keys.Select(async tournamentId =>
            {
                var tournamentResponse = await httpClient.GetStringAsync($"{DatabaseConnection}{TournamentsNode}{tournamentId}.json");
                var tournament = JsonConvert.DeserializeObject<TournamentDto>(tournamentResponse);
                return tournament;
            });

            tournaments = (await Task.WhenAll(tasks)).Where(t => t != null).ToList();

            return tournaments.ToList();
        }

        public async Task<bool> DeleteTournamentAsync(Guid tournamentUid)
        {
            var response = await _httpClient.DeleteAsync($"{DatabaseConnection}{TournamentsNode}{tournamentUid}.json");
            if (response.IsSuccessStatusCode)
            {
                return await DeleteTournamentByUidAndNullableListOfSummonerNamesInSummonerTournaments(tournamentUid, new List<string>());
            }
            else
            {
                return false;
            }
        }

        public async Task<TournamentResultDto> UpsertTournamentsHistoryAsync(TournamentResultDto tournamentResult)
        {
            TournamentResultDto tournamentResultUpdated = null;
            var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{TournamentsHistory}{tournamentResult.Uid}.json", tournamentResult);

            if (response.IsSuccessStatusCode)
            {
                tournamentResultUpdated = JsonConvert.DeserializeObject<TournamentResultDto>(await response.Content.ReadAsStringAsync());
            }

            return tournamentResultUpdated;
        }

        public async Task<bool> DeleteTournamentsHistoryAsync(Guid tournamentResultUid)
        {
            bool sucessfully = false;
            var response = await _httpClient.DeleteAsync($"{DatabaseConnection}{TournamentsHistory}{tournamentResultUid}.json");

            if (response.IsSuccessStatusCode)
            {
                sucessfully = true;
            }

            return sucessfully;
        }

        public async Task<TournamentResultDto> GetTournamentsHistoryAsync(Guid tournamentResultUid)
        {
            TournamentResultDto tournamentResultUpdated = null;
            var response = await _httpClient.GetAsync($"{DatabaseConnection}{TournamentsHistory}{tournamentResultUid}.json");

            if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != NullResponse)
            {
                tournamentResultUpdated = JsonConvert.DeserializeObject<TournamentResultDto>(await response.Content.ReadAsStringAsync());
            }

            return tournamentResultUpdated;
        }

        private async Task<TournamentDto> GetTournamentByUidAsync(Guid tournamentUid)
        {
            TournamentDto tournament = null;
            var response = await _httpClient.GetAsync($"{DatabaseConnection}{TournamentsNode}{tournamentUid}.json");
            if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != NullResponse)
            {
                tournament = JsonConvert.DeserializeObject<TournamentDto>(await response.Content.ReadAsStringAsync());
            }

            return tournament;
        }

        private async Task<bool> DeleteTournamentByUidAndNullableListOfSummonerNamesInSummonerTournaments(Guid tournamentUid, List<string> summonerNames)
        {
            try
            {
                if (summonerNames.Count > 0)
                {
                    foreach (var summonerName in summonerNames)
                    {
                        var summonerNameDatabase = summonerName.Replace("#", "*");
                        HttpResponseMessage response = await _httpClient.DeleteAsync($"{DatabaseConnection}/summonerTournaments/{summonerNameDatabase}/{tournamentUid}.json");
                        response.EnsureSuccessStatusCode();
                    }
                }
                else
                {
                    string url = $"{DatabaseConnection}/summonerTournaments.json";
                    string jsonResponse = await _httpClient.GetStringAsync(url);
                    var summonerTournaments = JObject.Parse(jsonResponse);
                    var tasks = summonerTournaments.Properties().Select(async summonerProperty =>
                    {
                        var summonerName = summonerProperty.Name;
                        var tournaments = summonerProperty.Value as JObject;
                        var tournamentProperty = tournaments?.Property(tournamentUid.ToString());

                        if (tournamentProperty != null)
                        {
                            string deleteUrl = $"{DatabaseConnection}/summonerTournaments/{summonerName}/{tournamentUid}.json";
                            HttpResponseMessage response = await _httpClient.DeleteAsync(deleteUrl);
                            response.EnsureSuccessStatusCode();
                        }
                    });

                    await Task.WhenAll(tasks);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task UpsertSummonerTournamentsAsync(Guid tournamentUid, List<string> summonerNames)
        {
            try
            {
                foreach (var summonerName in summonerNames)
                {
                    var summonerNameDatabase = summonerName.Replace("#", "*");
                    var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{SummonerTournamentsNode}{summonerNameDatabase}/{tournamentUid}.json", true);

                    response.EnsureSuccessStatusCode();

                    var sumonerTournamets = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

    }
}
