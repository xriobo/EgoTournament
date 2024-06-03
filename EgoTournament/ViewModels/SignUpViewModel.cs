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
            this._firebaseService = firebaseService;
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
                UserCredential userCredential = await _firebaseService.SignUp(Email, Password);
                await Shell.Current.DisplayAlert("Success", "Successfully signed up!", "Ok");
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (FirebaseAuthHttpException ex)
            {
                var fireBaseError = JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData);
                var toast = Toast.Make(fireBaseError.Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short);
                await toast.Show();
            }
            catch (Exception ex)
            {
                var toast = Toast.Make("Failed to sign up. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short);
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private async void SignInBtnTappedAsync(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}