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

        public Command SignUpBtn { get; }
        public Command SignInBtn { get; }

        public LoginViewModel(ICacheService cacheService, IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            _cacheService = cacheService;

            SignUpBtn = new Command(SignUpBtnTappedAsync);
            SignInBtn = new Command(SignInBtnTappedAsync);
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

        private async void SignInBtnTappedAsync(object obj)
        {
            try
            {
                SetCacheValues(await _firebaseService.SignIn(Email, Password));
                var toast = Toast.Make("Welcome!", CommunityToolkit.Maui.Core.ToastDuration.Short);
                await Shell.Current.GoToAsync($"//{nameof(ListingPage)}");
                Email = null;
                Password = null;
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

        private async void SignUpBtnTappedAsync(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private async void SetCacheValues(UserCredential userCredential)
        {
            await _cacheService.SetCurrentUserCredentialAsync(userCredential.User);
            var currentUser = await _firebaseService.GetUserByUid(userCredential.User.Uid);
            if (currentUser != null)
            {
                await _cacheService.SetCurrentUserAsync(currentUser);
            }
            
        }
    }
}