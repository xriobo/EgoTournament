using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IFirebaseService _firebaseService;
        private string email;
        private string password;

        public event PropertyChangedEventHandler PropertyChanged;

        public Command SignUpBtn { get; }
        public Command SignInBtn { get; }

        public LoginViewModel(IFirebaseService firebaseService)
        {
            this._firebaseService = firebaseService;
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
                UserCredential userCredential = await _firebaseService.SignIn(Email, Password);
                await SecureStorage.SetAsync(Globals.CURRENT_USER, JsonConvert.SerializeObject(userCredential.User));
                await Shell.Current.DisplayAlert("Success", "Successfully signed in!", "Ok");
                await Shell.Current.GoToAsync($"//{nameof(ListingPage)}");
            }
            catch (FirebaseAuthHttpException ex)
            {
                var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
                var toast = Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short);
                await toast.Show();
            }
            catch (Exception ex)
            {
                var toast = Toast.Make("Failed to sign in. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short);
                Console.WriteLine(ex.ToString());
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
    }
}