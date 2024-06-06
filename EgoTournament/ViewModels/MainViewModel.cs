using EgoTournament.Models;
using EgoTournament.Services;

namespace EgoTournament.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IFirebaseService _firebaseService;
        private readonly ICacheService _cacheService;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        public ObservableCollection<TournamentDto> tournaments;

        public MainViewModel(ICacheService cacheService, IFirebaseService firebaseService)
        {
            this._firebaseService = firebaseService;
            this._cacheService = cacheService;
        }

        public async void LoadData()
        {
            var currentUser = await _cacheService.GetCurrentUserAsync();
            Tournaments = new ObservableCollection<TournamentDto>(currentUser.Tournaments);
        }

        [RelayCommand]
        private void OnRefreshing()
        {
            IsRefreshing = true;

            try
            {
                LoadData();
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
