using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using Firebase.Auth;
using Newtonsoft.Json;

namespace EgoTournament.ViewModels
{
    /// <summary>
    /// Prompt view model class.
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class PromptViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the accept command.
        /// </summary>
        /// <value>
        /// The accept command.
        /// </value>
        public IRelayCommand AcceptCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>
        /// The cancel command.
        /// </value>
        public IRelayCommand CancelCommand { get; }

        /// <summary>
        /// The firebase service.
        /// </summary>
        private readonly IFirebaseService _firebaseService;

        /// <summary>
        /// The cache service.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// The navigation service.
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// The method type.
        /// </summary>
        private readonly MethodType _methodType;

        /// <summary>
        /// List of <see cref="TournamentDto"/> without the tournament to be updated.
        /// </summary>
        private readonly List<TournamentDto> _tournaments;

        /// <summary>
        /// The promptEntry binding.
        /// </summary>
        [ObservableProperty]
        private string promptEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptViewModel" /> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="firebaseService">The firebase service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="methodType">The method type.</param>
        /// <param name="tournaments">List of <see cref="TournamentDto" /> without the tournament to be updated.</param>
        public PromptViewModel(ICacheService cacheService, IFirebaseService firebaseService, INavigationService navigationService, MethodType methodType, List<TournamentDto> tournaments = null)
        {
            _cacheService = cacheService;
            _firebaseService = firebaseService;
            _methodType = methodType;
            _navigationService = navigationService;
            _tournaments = tournaments;

            AcceptCommand = new RelayCommand(OnAcceptClicked);
            AcceptCommand = new RelayCommand(OnAcceptClicked);
        }

        /// <summary>
        /// Called when [accept clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void OnAcceptClicked()
        {
            try
            {
                if (!string.IsNullOrEmpty(promptEntry))
                {
                    var cacheUser = await _cacheService.GetCurrentUserCredentialAsync();
                    var userCredential = await _firebaseService.SignIn(cacheUser.Info.Email, promptEntry);
                    switch (_methodType)
                    {
                        case MethodType.Profile:
                            this.DeleteProfile(userCredential);
                            break;
                        case MethodType.Main:
                            this.DeleteTournament(userCredential, _tournaments);
                            break;
                    }
                }
                else
                {
                    await Toast.Make("Insert password.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
            catch (FirebaseAuthHttpException ex)
            {
                await Toast.Make(JsonConvert.DeserializeObject<FirebaseErrorDto>(ex.ResponseData).Error.Message.Replace("_", " "), CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                await _navigationService.PopModalAsync();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to load profile. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                await _navigationService.PopModalAsync();
            }
        }

        /// <summary>
        /// Called when [cancel clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await _navigationService.PopModalAsync();
        }

        /// <summary>
        /// Deletes the profile.
        /// </summary>
        /// <param name="userCredential">The user credential.</param>
        private async void DeleteProfile(UserCredential userCredential)
        {
            await _firebaseService.DeleteUserAndUserCredential(userCredential.User.Uid);
            await userCredential.User.DeleteAsync();
            await Shell.Current.DisplayAlert("Removed", "The account has been successfully deleted.", "OK");
            _cacheService.Logout();
            await _navigationService.PopModalAsync();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        /// <summary>
        /// Deletes the tournament.
        /// </summary>
        /// <param name="userCredential">The user credential.</param>
        /// <param name="tournaments">The tournaments.</param>
        private async void DeleteTournament(UserCredential userCredential, List<TournamentDto> tournaments)
        {
            if (tournaments != null)
            {
                var currentUser = await _firebaseService.GetUserByUid(userCredential.User.Uid);
                currentUser.Tournaments = tournaments;
                var userUpdated = await _firebaseService.PutUser(currentUser);
                if (userUpdated != null)
                {
                    await _cacheService.SetCurrentUserAsync(userUpdated);
                    await Toast.Make("Tournament deleted.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
                else
                {
                    await Toast.Make("Elimination tournament error. Try again later...", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }


                await _navigationService.PopModalAsync();
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
