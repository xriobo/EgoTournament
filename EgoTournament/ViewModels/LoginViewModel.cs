using EgoTournament.Models;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IFirebaseService _firebaseService;
        private readonly INavigation _navigation;
        private readonly CurrentUserStore _currentUserStore;
        private string userEmail;
        private string userPassword;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public Command SignUpBtn { get; }
        public Command SignInBtn { get; }

        public LoginViewModel(INavigation navigation, IFirebaseService firebaseService, CurrentUserStore currentUserStore)
        {
            this._navigation = navigation;
            this._firebaseService = firebaseService;
            this._currentUserStore = currentUserStore;
            SignUpBtn = new Command(SignUpBtnTappedAsync);
            SignInBtn = new Command(SignInBtnTappedAsync);
        }

        public string UserEmail
        {
            get => userEmail; set
            {
                userEmail = value;
                RaisePropertyChanged("UserEmail");
            }
        }

        public string UserPassword
        {
            get => userPassword; set
            {
                userPassword = value;
                RaisePropertyChanged("UserPassword");
            }
        }

        private async void SignInBtnTappedAsync(object obj)
        {
            if (await _firebaseService.SignIn(UserEmail, UserPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Successfully signed in!", "Ok");
                await this._navigation.PushAsync(new ListingPage(this._currentUserStore));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to sign in. Please try again later.", "Ok");
            }
        }

        private async void SignUpBtnTappedAsync(object obj)
        {
            await this._navigation.PushAsync(new SignUpPage(this._firebaseService));
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}