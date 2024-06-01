using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    internal class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly INavigation _navigation;
        private readonly IFirebaseService _firebaseService;
        private string userEmail;
        private string password;

        public event PropertyChangedEventHandler PropertyChanged;

        public SignUpViewModel(INavigation navigation, IFirebaseService firebaseService)
        {
            this._navigation = navigation;
            this._firebaseService = firebaseService;
            SignUpUser = new Command(SignUpTappedAsync);
        }

        public string UserEmail
        {
            get => userEmail;
            set
            {
                userEmail = value;
                RaisePropertyChanged("UserEmail");
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
            if (await _firebaseService.SignUp(UserEmail, Password))
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Successfully signed up!", "Ok");
                await this._navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to sign up. Please try again later.", "Ok");
            }
        }
    }
}