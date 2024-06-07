using CommunityToolkit.Maui.Alerts;
using EgoTournament.Behaviors;
using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Models.Enums;
using EgoTournament.Models.Firebase;
using EgoTournament.Services;
using System.Windows.Input;

namespace EgoTournament.ViewModels
{
    /// <summary>
    /// Profile view model class.
    /// </summary>
    /// <seealso cref="BindableObject" />
    public partial class ProfileViewModel : BindableObject
    {
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
        /// The user credentials.
        /// </summary>
        private FirebaseUserDto userCredentials;

        /// <summary>
        /// The current user.
        /// </summary>
        private UserDto currentUser;

        /// <summary>
        /// My entry text.
        /// </summary>
        private string _summonerNameEntryText;

        /// <summary>
        /// The is entry enabled.
        /// </summary>
        private bool _isEntryEnabled;

        /// <summary>
        /// The is validation message visible
        /// </summary>
        private bool _isValidationMessageVisible;

        /// <summary>
        /// The validation message.
        /// </summary>
        private string _validationMessage;

        /// <summary>
        /// The is rolce picker visible.
        /// </summary>
        private bool _isRolePickerEnable;

        /// <summary>
        /// The selected item.
        /// </summary>
        private string _selectedItem;

        /// <summary>
        /// The is label enable entry visible.
        /// </summary>
        private bool _isLabelEnableEntryVisible;

        /// <summary>
        /// The is checked property.
        /// </summary>
        private bool _modifyNameChecked;

        /// <summary>
        /// The is CheckBox enable.
        /// </summary>
        private bool _isCheckBoxEnable;

        /// <summary>
        /// The is CheckBox visible.
        /// </summary>
        private bool _isCheckBoxVisible;

        /// <summary>
        /// The is button visible.
        /// </summary>
        private bool _isButtonVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileViewModel"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="firebaseService">The firebase service.</param>
        /// <param name="navigationService">The navigation service.</param>
        public ProfileViewModel(ICacheService cacheService, IFirebaseService firebaseService, INavigationService navigationService)
        {
            _cacheService = cacheService;
            _firebaseService = firebaseService;
            _navigationService = navigationService;
            LoadScreenData(currentUser);
            RoleValues = new ObservableCollection<string>(Enum.GetNames(typeof(RoleType)).ToList());
            SummonerNameEntryTextChangedCommand = new Command(OnSummonerNameTextChanged);
            SaveCommand = new RelayCommand(OnSaveClicked);
            LogoutCommand = new RelayCommand(OnLogoutClicked);
            DeleteAccountCommand = new RelayCommand(OnDeleteAccountClicked);
            CheckBoxChangedCommand = new Command<bool>(OnCheckBoxChanged);
        }

        /// <summary>
        /// Called when [navigated to asynchronous].
        /// </summary>
        public async Task OnNavigatedToAsync()
        {
            userCredentials = await _cacheService.GetCurrentUserCredentialAsync();
            if (userCredentials != null)
            {
                currentUser = await _cacheService.GetCurrentUserAsync();
                LoadScreenData(currentUser);
                await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SummonerNameTextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnSummonerNameTextChanged()
        {
            if (IsValidationMessageVisible)
            {
                IsValidationMessageVisible = false;
            }
        }

        /// <summary>
        /// Called when [save clicked].
        /// </summary>
        private async void OnSaveClicked()
        {
            try
            {
                var entryText = SummonerNameEntryText;
                if (!string.IsNullOrEmpty(entryText))
                {
                    if (!ModifyNameChecked)
                    {
                        var role = Enum.Parse<RoleType>(SelectedItem.ToString(), true);
                        if (Validations.SummonerName(entryText))
                        {
                            if (currentUser == null)
                            {
                                currentUser = new UserDto()
                                {
                                    Role = role,
                                    SummonerName = entryText,
                                    Uid = userCredentials.Uid,
                                    Email = userCredentials.Info.Email,
                                };

                                await _firebaseService.CreateUser(currentUser);
                                await _cacheService.SetCurrentUserAsync(currentUser);
                                LoadScreenData(currentUser);
                            }
                            else
                            {
                                await Toast.Make("Error: User already exists in the database..", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                            }
                        }
                        else
                        {
                            ValidationMessage = Globals.SUMMONERNAME_VALIDATION_ERROR_MESSAGE;
                            IsValidationMessageVisible = true;
                            await Toast.Make($"SummonerName invalid: {entryText}", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                        }
                    }
                    else
                    {
                        currentUser.SummonerName = entryText;
                        if (await Shell.Current.DisplayAlert("WARNING", "Do you confirm that you want to modify the summonerName?", "YES", "NO")
                                && await _firebaseService.PutUser(currentUser) != null)
                        {
                            await _cacheService.SetCurrentUserAsync(currentUser);
                            await Toast.Make("Updated successfully.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                        }
                        else
                        {
                            currentUser = await _cacheService.GetCurrentUserAsync();
                        }

                        LoadScreenData(currentUser);
                    }
                }
                else
                {
                    await Toast.Make("Insert a value.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await Toast.Make("Failed to load profile. Please try again later.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            }
        }

        /// <summary>
        /// Called when [logout clicked].
        /// </summary>
        private async void OnLogoutClicked()
        {
            _cacheService.Logout();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("Goodbye!", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }

        /// <summary>
        /// Called when [delete account clicked].
        /// </summary>
        private async void OnDeleteAccountClicked()
        {
            await _navigationService.PushModalAsync(new PromptPage(_cacheService, _firebaseService, _navigationService, MethodType.Profile));
        }

        /// <summary>
        /// Raises the <see cref="E:EnableSummonerNameCheckedChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="CheckedChangedEventArgs"/> instance containing the event data.</param>
        private async void OnCheckBoxChanged(bool isChecked)
        {
            if (isChecked && await Shell.Current.DisplayAlert("WARNING", "If the summoner name is modified, the assigned tournaments will change to the new name.", "Ok", "Cancel"))
            {
                IsSummonerNameEntryEnable = true;
                IsSaveButtonVisible = true;
            }
            else
            {
                ModifyNameChecked = false;
                IsSummonerNameEntryEnable = false;
                IsSaveButtonVisible = false;
            }
        }

        /// <summary>
        /// Loads the screen data.
        /// </summary>
        /// <param name="user">The user.</param>
        private void LoadScreenData(UserDto user)
        {
            if (user != null)
            {
                currentUser = user;
                SummonerNameEntryText = user.SummonerName;
                IsSummonerNameEntryEnable = false;
                SelectedItem = user.Role.ToString();
                IsRolePickerEnable = false;
                IsSaveButtonVisible = false;
                IsLabelEnableEntryVisible = true;
                IsCheckBoxVisible = true;
                IsCheckBoxEnable = true;
                ModifyNameChecked = false;
            }
            else
            {
                SelectedItem = default(RoleType).ToString();
                SummonerNameEntryText = null;
                IsSummonerNameEntryEnable = true;
                IsRolePickerEnable = true;
                IsSaveButtonVisible = true;
            }
        }

        /// <summary>
        /// Gets the CheckBox changed command.
        /// </summary>
        /// <value>
        /// The CheckBox changed command.
        /// </value>
        public ICommand CheckBoxChangedCommand { get; }

        /// <summary>
        /// Gets the summoner name entry text changed command.
        /// </summary>
        /// <value>
        /// The summoner name entry text changed command.
        /// </value>
        public ICommand SummonerNameEntryTextChangedCommand { get; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <value>
        /// The save command.
        /// </value>
        public IRelayCommand SaveCommand { get; }

        /// <summary>
        /// Gets the logout command.
        /// </summary>
        /// <value>
        /// The logout command.
        /// </value>
        public IRelayCommand LogoutCommand { get; }

        /// <summary>
        /// Gets the delete account command.
        /// </summary>
        /// <value>
        /// The delete account command.
        /// </value>
        public IRelayCommand DeleteAccountCommand { get; }

        /// <summary>
        /// Gets or sets the summoner name entry text.
        /// </summary>
        /// <value>
        /// The summoner name entry text.
        /// </value>
        public string SummonerNameEntryText
        {
            get { return _summonerNameEntryText; }
            set
            {
                if (_summonerNameEntryText != value)
                {
                    _summonerNameEntryText = value;
                    OnPropertyChanged(nameof(SummonerNameEntryText));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is entry enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is entry enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsSummonerNameEntryEnable
        {
            get { return _isEntryEnabled; }
            set
            {
                if (_isEntryEnabled != value)
                {
                    _isEntryEnabled = value;
                    OnPropertyChanged(nameof(IsSummonerNameEntryEnable));
                }
            }
        }

        /// <summary>
        /// The role values.
        /// </summary>
        public ObservableCollection<string> RoleValues { get; }

        public bool IsValidationMessageVisible
        {
            get => _isValidationMessageVisible;
            set
            {
                _isValidationMessageVisible = value;
                OnPropertyChanged();
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is button visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsRolePickerEnable
        {
            get { return _isRolePickerEnable; }
            set
            {
                if (_isRolePickerEnable != value)
                {
                    _isRolePickerEnable = value;
                    OnPropertyChanged(nameof(IsRolePickerEnable));
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is label enable entry visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is label enable entry visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLabelEnableEntryVisible
        {
            get => _isLabelEnableEntryVisible;
            set
            {
                _isLabelEnableEntryVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is CheckBox enable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is CheckBox enable; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckBoxEnable
        {
            get => _isCheckBoxEnable;
            set
            {
                _isCheckBoxEnable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool ModifyNameChecked
        {
            get => _modifyNameChecked;
            set
            {
                _modifyNameChecked = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is CheckBox visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is CheckBox visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckBoxVisible
        {
            get => _isCheckBoxVisible;
            set
            {
                _isCheckBoxVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is button visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsSaveButtonVisible
        {
            get { return _isButtonVisible; }
            set
            {
                if (_isButtonVisible != value)
                {
                    _isButtonVisible = value;
                    OnPropertyChanged(nameof(IsSaveButtonVisible));
                }
            }
        }
    }
}
