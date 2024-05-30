namespace EgoTournament.ExternalServices.Implementation
{
    using RestSharp;
    using System.Threading.Tasks;

    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly RestClient _client;
        private readonly string _apiKey;

        public FirebaseAuthService()
        {
            _apiKey = FirebaseConfig.ApiKey;
            _client = new RestClient("https://identitytoolkit.googleapis.com/v1/");
        }

        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            var request = new RestRequest("accounts:signInWithPassword?key=" + _apiKey, Method.Post);
            request.AddJsonBody(new
            {
                email,
                password,
                returnSecureToken = true
            });

            var response = await _client.ExecuteAsync<FirebaseAuthResponse>(request);

            if (response.IsSuccessful)
            {
                return response.Data.IdToken;
            }
            else
            {
                throw new Exception(response.Content);
            }
        }

        private class FirebaseAuthResponse
        {
            public string IdToken { get; set; }
        }
    }
}