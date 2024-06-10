using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly IFirebaseService _firebaseService;
        private string email;
        private string password;
        public Command SignInBtn { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public SignUpViewModel(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            SignUpUser = new Command(SignUpTappedAsync);
            SignInBtn = new Command(SignInBtnTappedAsync);
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

        public Command SignUpUser { get; }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        private async void SignUpTappedAsync(object obj)
        {
            try
            {
                await _firebaseService.SignUp(Email, Password);
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
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
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                Password = null;
                Email = null;
            }
        }

        private async void SignInBtnTappedAsync(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}