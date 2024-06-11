using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ICacheService _cacheService;
        private string email = string.Empty;
        private string password = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IAsyncRelayCommand SignUpBtn { get; }
        public IAsyncRelayCommand SignInBtn { get; }

        public LoginViewModel(ICacheService cacheService, IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            _cacheService = cacheService;

            SignUpBtn = new AsyncRelayCommand(SignUpBtnTappedAsync);
            SignInBtn = new AsyncRelayCommand(SignInBtnTappedAsync);
        }

        public string Email
        {
            get => email; set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        public string Password
        {
            get => password; set
            {
                password = value;
                RaisePropertyChanged("Password");
            }
        }

        private async Task SignInBtnTappedAsync()
        {
            try
            {
                var userCredentials = await _firebaseService.SignInAsync(Email, Password);
                await _cacheService.SetCurrentUserCredentialAsync(userCredentials.User);
                var currentUser = await _firebaseService.GetUserByUidAsync(userCredentials.User.Uid);
                Email = null;
                Password = null;
                await Toast.Make("Welcome!", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                if (currentUser != null)
                {
                    await _cacheService.SetCurrentUserAsync(currentUser);
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("PROFILE", "Set up your profile to continue.", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
                }       
            }
            catch (FirebaseAuthHttpException ex)
            {
                var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
                await Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to sign in. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }

        private async Task SignUpBtnTappedAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private async Task SetCacheValues(UserCredential userCredential)
        {
            await _cacheService.SetCurrentUserCredentialAsync(userCredential.User);
            var currentUser = await _firebaseService.GetUserByUidAsync(userCredential.User.Uid);
            if (currentUser != null)
            {
                await _cacheService.SetCurrentUserAsync(currentUser);
            }
            
        }
    }
}