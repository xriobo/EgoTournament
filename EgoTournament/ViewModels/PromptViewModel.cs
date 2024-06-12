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
    public partial class PromptViewModel : BaseViewModel, IQueryAttributable
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
        private MethodType _methodType;

        /// <summary>
        /// The tournament uid to delete.
        /// </summary>
        private Guid _tournamentUidToDelete;

        /// <summary>
        /// The tournament dto.
        /// </summary>
        private TournamentDto _tournamentDto;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptViewModel" /> class.
        /// </summary>
        public PromptViewModel()
        {
            _cacheService = App.Services.GetService<ICacheService>();
            _firebaseService = App.Services.GetService<IFirebaseService>();

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
                    var cacheUserCredential = await _cacheService.GetCurrentUserCredentialAsync();
                    var userCredential = await _firebaseService.SignInAsync(cacheUserCredential.Info.Email, PromptEntry);
                    switch (_methodType)
                    {
                        case MethodType.Profile:
                            await DeleteProfile(userCredential);
                            break;
                        case MethodType.Main:
                            await DeleteTournament(userCredential, _tournamentUidToDelete);
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
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        /// <summary>
        /// Deletes the tournament.
        /// </summary>
        /// <param name="userCredential">The user credential.</param>
        /// <param name="tournaments">The tournaments.</param>
        private async Task DeleteTournament(UserCredential userCredential, Guid tournamentUidToDelete)
        {
            if (tournamentUidToDelete != Guid.Empty)
            {
                var currentUser = await _firebaseService.GetUserByUidAsync(userCredential.User.Uid);
                if (currentUser != null) 
                {
                    var tournamentDeleted = await _firebaseService.DeleteTournamentAsync(tournamentUidToDelete);
                    if (tournamentDeleted)
                    {
                        currentUser = this.DeleteUserCacheTournament(currentUser, tournamentUidToDelete);
                        var userUpdated = await _firebaseService.UpsertUserAsync(currentUser);
                        await _cacheService.SetCurrentUserAsync(userUpdated);
                        await Toast.Make("Tournament deleted.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }
                    else
                    {
                        await Toast.Make("Elimination tournament error. Try again later...", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    }
                }

                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }

        /// <summary>
        /// Deletes the user cache tournament.
        /// </summary>
        /// <param name="userToUpdate">The user to update.</param>
        /// <param name="tournamentUidToDelete">The tournament uid to delete.</param>
        /// <returns>A <see cref="UserDto"/>.</returns>
        private UserDto DeleteUserCacheTournament(UserDto userToUpdate, Guid tournamentUidToDelete)
        {
            if (userToUpdate.Tournaments != null && userToUpdate.Tournaments.Any(x => x.Uid == tournamentUidToDelete))
            {
                userToUpdate.Tournaments.Remove(userToUpdate.Tournaments.First(x => x.Uid == tournamentUidToDelete));
            }

            if (userToUpdate.TournamentUids != null && userToUpdate.TournamentUids.Contains(tournamentUidToDelete.ToString()))
            {
                userToUpdate.TournamentUids.Remove(tournamentUidToDelete.ToString());
            }

            return userToUpdate;
        }

        /// <summary>
        /// Applies the query attributes.
        /// </summary>
        /// <param name="query">The query.</param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.Any()) return;
            _tournamentDto = query[nameof(TournamentDto)] as TournamentDto;
            var methodType = query[nameof(MethodType)] as MethodType?;
            if (methodType != null)
            {
                _methodType = methodType.Value;
            }

            if (_tournamentDto.Uid != Guid.Empty)
            {
                _tournamentUidToDelete = _tournamentDto.Uid;
            }
        }
    }
}
