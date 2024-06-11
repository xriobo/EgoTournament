using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using Firebase.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace EgoTournament.Services.Implementations
{
    internal class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly HttpClient _httpClient;
        private const string DatabaseConnection = "https://egotournament1-default-rtdb.europe-west1.firebasedatabase.app";
        private const string UsersNode = "/users/";
        private const string TournamentsNode = "/tournaments/";
        private const string SummonerTournamentsNode = "/summonerTournaments/";
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

        public async Task<TournamentDto> UpsertTournamentAsync(TournamentDto tournament, bool updateSummonerTournamentsRelation = true)
        {
            TournamentDto tournamentUpdated = null;
            var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{TournamentsNode}{tournament.Uid}.json", tournament);

            if (response.IsSuccessStatusCode)
            {
                tournamentUpdated = JsonConvert.DeserializeObject<TournamentDto>(await response.Content.ReadAsStringAsync());
            }

            if (updateSummonerTournamentsRelation)
            {
                await UpsertSummonerTournamentsAsync(tournament.Uid, tournament.SummonerNames);
            }

            return tournamentUpdated;
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

        private async Task UpsertSummonerTournamentsAsync(Guid tournamentUid, List<string> summonerNames)
        {
            foreach (var summonerName in summonerNames)
            {
                await _httpClient.PutAsJsonAsync($"{DatabaseConnection}{SummonerTournamentsNode}{summonerName}/{tournamentUid}.json", true);
            }
        }

        public async Task<List<TournamentDto>> GetTournamentsBySummonerNameAsync(string summonerName)
        {
            var httpClient = new HttpClient();
            var tournaments = new List<TournamentDto>();
            var summonerTournamentsResponse = await httpClient.GetStringAsync($"{DatabaseConnection}{SummonerTournamentsNode}{summonerName}.json");

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

        public async Task DeleteTournamentAsync(Guid tournamentUid)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{DatabaseConnection}/summonerTournaments.json";
                string jsonResponse = await httpClient.GetStringAsync(url);
                var summonerTournaments = JObject.Parse(jsonResponse);

                var tasks = summonerTournaments.Properties().Select(async summonerProperty =>
                {
                    var summonerName = summonerProperty.Name;
                    var tournaments = summonerProperty.Value as JObject;
                    var tournamentProperty = tournaments?.Property(tournamentUid.ToString());

                    if (tournamentProperty != null)
                    {
                        string deleteUrl = $"{DatabaseConnection}/summonerTournaments/{summonerName}/{tournamentUid}.json";
                        HttpResponseMessage response = await httpClient.DeleteAsync(deleteUrl);
                        response.EnsureSuccessStatusCode();
                    }
                });

                await Task.WhenAll(tasks);
            }
        }
    }
}
