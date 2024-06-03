using EgoTournament.Models;
using Firebase.Auth;
using System.Net.Http.Json;
using System.Text;
using System.Threading;

namespace EgoTournament.Services.Implementations
{
    internal class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly HttpClient _httpClient;
        private const string DatabaseConnection = "https://egotournament1-default-rtdb.europe-west1.firebasedatabase.app";

        public FirebaseService(FirebaseAuthClient authClient)
        {
            this._authClient = authClient;
            _httpClient = new HttpClient();
        }

        public Task CreateTournament(TournamentDto tournament)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExists(string userUid)
        {
            var response = await _httpClient.GetAsync($"{DatabaseConnection}/users/{userUid}.json");
            return response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != "null";
        }

        public async Task CreateUser(UserDto user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{DatabaseConnection}/users/{user.Uid}.json", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetUserByUid(string userUid)
        {
            var response = await _httpClient.GetAsync($"{DatabaseConnection}/users/{userUid}.json");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<UserCredential> SignIn(string email, string password)
        {
            return await _authClient.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task<UserCredential> SignUp(string email, string password)
        {
            return await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);
        }
    }
}
