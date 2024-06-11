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
        /// The PromptEntry binding.
        /// </summary>
        [ObservableProperty]
        public string promptEntry;

        /// <summary>
        /// Gets the accept command.
        /// </summary>
        /// <value>
        /// The accept command.
        /// </value>
        public IAsyncRelayCommand AcceptCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>
        /// The cancel command.
        /// </value>
        public IAsyncRelayCommand CancelCommand { get; }

        /// <summary>
        /// The firebase service.
        /// </summary>
        private readonly IFirebaseService _firebaseService;

        /// <summary>
        /// The cache service.
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// The method type.
        /// </summary>
        private readonly MethodType _methodType;

        /// <summary>
        /// List of <see cref="TournamentDto"/> without the tournament to be updated.
        /// </summary>
        private readonly List<TournamentDto> _tournaments;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptViewModel" /> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="firebaseService">The firebase service.</param>
        /// <param name="methodType">The method type.</param>
        /// <param name="tournaments">List of <see cref="TournamentDto" /> without the tournament to be updated.</param>
        public PromptViewModel(ICacheService cacheService, IFirebaseService firebaseService, MethodType methodType, List<TournamentDto> tournaments = null)
        {
            _cacheService = cacheService;
            _firebaseService = firebaseService;
            _methodType = methodType;
            _tournaments = tournaments;

            AcceptCommand = new AsyncRelayCommand(OnAcceptClicked);
            CancelCommand = new AsyncRelayCommand(OnCancelClicked);
        }

        /// <summary>
        /// Called when [accept clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async Task OnAcceptClicked()
        {
            try
            {
                if (!string.IsNullOrEmpty(PromptEntry))
                {
                    var cacheUser = await _cacheService.GetCurrentUserCredentialAsync();
                    var userCredential = await _firebaseService.SignInAsync(cacheUser.Info.Email, PromptEntry);
                    switch (_methodType)
                    {
                        case MethodType.Profile:
                            await DeleteProfile(userCredential);
                            break;
                        case MethodType.Main:
                            await DeleteTournament(userCredential, _tournaments);
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

                var navigation = App.Current.MainPage.Navigation;
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to load profile. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// Called when [cancel clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async Task OnCancelClicked()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Deletes the profile.
        /// </summary>
        /// <param name="userCredential">The user credential.</param>
        private async Task DeleteProfile(UserCredential userCredential)
        {
            await _firebaseService.DeleteUserAndUserCredentialAsync(userCredential.User.Uid);
            await userCredential.User.DeleteAsync();
            await Shell.Current.DisplayAlert("Removed", "The account has been successfully deleted.", "OK");
            _cacheService.Logout();
            await App.Current.MainPage.Navigation.PopModalAsync();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        /// <summary>
        /// Deletes the tournament.
        /// </summary>
        /// <param name="userCredential">The user credential.</param>
        /// <param name="tournaments">The tournaments.</param>
        private async Task DeleteTournament(UserCredential userCredential, List<TournamentDto> tournaments)
        {
            if (tournaments != null)
            {
                var currentUser = await _firebaseService.GetUserByUidAsync(userCredential.User.Uid);
                currentUser.Tournaments = tournaments;
                var userUpdated = await _firebaseService.UpsertUserAsync(currentUser);
                if (userUpdated != null)
                {
                    await _cacheService.SetCurrentUserAsync(userUpdated);
                    await Toast.Make("Tournament deleted.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
                else
                {
                    await Toast.Make("Elimination tournament error. Try again later...", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }


                await App.Current.MainPage.Navigation.PopModalAsync();
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
