using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public class SignUpViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly IFirebaseService _firebaseService;
        private string email;
        private string password;
        public IAsyncRelayCommand SignInBtn { get; }
        public IAsyncRelayCommand SignUpUser { get; }

        public SignUpViewModel(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            SignUpUser = new AsyncRelayCommand(SignUpTappedAsync);
            SignInBtn = new AsyncRelayCommand(SignInBtnTappedAsync);
        }

        public string Email
        {
            get => email;
            set
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

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private async Task SignUpTappedAsync()
        {
            try
            {
                await _firebaseService.SignUpAsync(Email, Password);
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                await Toast.Make("Welcome! Sign in.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            catch (FirebaseAuthHttpException ex)
            {
                var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
                await Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            catch (Exception ex)
            {
                await Toast.Make("Failed to sign up. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            finally
            {
                Password = null;
                Email = null;
            }
        }

        private async Task SignInBtnTappedAsync()
        {
            Password = null;
            Email = null;
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}