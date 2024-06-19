using CommunityToolkit.Maui.Alerts;
using EgoTournament.Adapters;
using EgoTournament.Common;
using EgoTournament.Models.Riot;
using EgoTournament.Models.Views;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    /// <summary>
    /// Search Summoner View Model class.
    /// </summary>
    /// <seealso cref="EgoTournament.ViewModels.BaseViewModel" />
    public partial class SearchSummonerViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>
        /// The search command.
        /// </value>
        public IAsyncRelayCommand SearchCommand { get; }

        /// <summary>
        /// Gets the summoner name entry text changed command.
        /// </summary>
        /// <value>
        /// The summoner name entry text changed command.
        /// </value>
        public IRelayCommand SummonerNameEntryTextChangedCommand { get;  }

        /// <summary>
        /// The riot service.
        /// </summary>
        private IRiotService  _riotService;

        /// <summary>
        /// The summoner view.
        /// </summary>
        [ObservableProperty]
        public SummonerView summonerView;

        /// <summary>
        /// The matches view model.
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<MatchView> matchesViewModel;

        /// <summary>
        /// The count matches.
        /// </summary>
        private const int CountMatches = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSummonerViewModel"/> class.
        /// </summary>
        public SearchSummonerViewModel()
        {
            SearchCommand = new AsyncRelayCommand(SearchSummonerData);
            SummonerNameEntryTextChangedCommand = new RelayCommand(OnSummonerNameTextChanged);

           _riotService = App.Services.GetService<IRiotService>(); ;
        }

        /// <summary>
        /// Gets the summoner data.
        /// </summary>
        /// <param name="summonerName">Name of the summoner.</param>
        public async Task GetSummonerData(string summonerName)
        {
            var gameName = summonerName.Split('#')[0];
            var tagLine = summonerName.Split('#')[1];
            if (SearchSummonerDto.SummonerWithMatchesDto == null
                || !gameName.Equals(SearchSummonerDto.SummonerWithMatchesDto.SummonerDto.Name, StringComparison.InvariantCultureIgnoreCase)
                || (DateTime.Now - SearchSummonerDto.ExtractSummonerDateTime) > new TimeSpan(0, 0, 30))
            {
                SummonerWithMatchesDto info = await _riotService.GetSummonerWithMachesBySummonerNameAndTagLine(gameName, tagLine, CountMatches);
                Tuple<SummonerView, List<MatchView>> tupleSummmonerMatches = info.ToSearchSummonerViewModels();
                SummonerView = tupleSummmonerMatches.Item1;
                MatchesViewModel = new ObservableCollection<MatchView>(tupleSummmonerMatches.Item2);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SummonerNameTextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnSummonerNameTextChanged()
        {
            SummonerView = null;
            MatchesViewModel = new ObservableCollection<MatchView>();
            if (IsValidationMessageVisible)
            {
                IsValidationMessageVisible = false;
            }
        }

        /// <summary>
        /// Searches the summoner data.
        /// </summary>
        private async Task SearchSummonerData()
        {
            var entryText = SummonerNameEntryText;
            if (string.IsNullOrEmpty(entryText))
            {
                await Toast.Make("Insert a value.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            else
            {
                if (Validations.SummonerName(entryText))
                {
                    await GetSummonerData(entryText);
                }
                else
                {
                    ValidationMessage = Globals.SUMMONERNAME_VALIDATION_ERROR_MESSAGE;
                    IsValidationMessageVisible = true;
                    await Toast.Make($"SummonerName invalid: {entryText}", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                }
            }
        }

        /// <summary>
        /// The validation message.
        /// </summary>
        private string _validationMessage;

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        /// <value>
        /// The validation message.
        /// </value>
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
        /// The is validation message visible
        /// </summary>
        private bool _isValidationMessageVisible;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation message visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is validation message visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationMessageVisible
        {
            get => _isValidationMessageVisible;
            set
            {
                _isValidationMessageVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// My entry text.
        /// </summary>
        private string _summonerNameEntryText;

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
    }
}