using EgoTournament.Models;
using Firebase.Auth;
using Newtonsoft.Json;
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
        private const string NullResponse = "null";

        public FirebaseService(FirebaseAuthClient authClient)
        {
            this._authClient = authClient;
            _httpClient = new HttpClient();
        }

        public async Task CreateUser(UserDto user)
        {
            var response = await _httpClient.PostAsJsonAsync($"{DatabaseConnection}/users/{user.Uid}.json", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDto> GetUserByUid(string userUid)
        {
            UserDto userDto = null;
            var response = await _httpClient.GetAsync($"{DatabaseConnection}/users/{userUid}.json");
            if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync() != NullResponse)
            {
                userDto = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
            }

            return userDto;
        }

        public async Task<UserDto> PutUser(UserDto userDto)
        {
            UserDto userUpdated = null;
            var response = await _httpClient.PutAsJsonAsync($"{DatabaseConnection}/users/{userDto.Uid}.json", userDto);
            
            if (response.IsSuccessStatusCode)
            {
                userUpdated = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
            }

            return userUpdated;
        }

        public async Task DeleteUserAndUserCredential(string userUid)
        {
            var response = await _httpClient.DeleteAsync($"{DatabaseConnection}/users/{userUid}.json");
            response.EnsureSuccessStatusCode();
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
