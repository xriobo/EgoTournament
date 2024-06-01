using EgoTournament.Models;
using Firebase.Auth;

namespace EgoTournament.Services.Implementations
{
    internal class FirebaseService : IFirebaseService
    {
        private readonly FirebaseAuthClient _authClient;
        private readonly CurrentUserStore _currentUserStore;

        public FirebaseService(FirebaseAuthClient authClient, CurrentUserStore currentUserStore)
        {
            this._authClient = authClient;
            this._currentUserStore = currentUserStore;
        }

        public async Task<bool> SignIn(string email, string password)
        {
            try
            {
                UserCredential userCredential = await _authClient.SignInWithEmailAndPasswordAsync(
                    email,
                    password);

                _currentUserStore.CurrentUser = userCredential.User;

                await Shell.Current.GoToAsync("//Main");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SignUp(string email, string password)
        {
            try
            {
                UserCredential userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                _currentUserStore.CurrentUser = userCredential.User;

                await Shell.Current.GoToAsync("//Main");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
