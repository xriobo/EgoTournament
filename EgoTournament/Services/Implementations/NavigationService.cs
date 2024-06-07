namespace EgoTournament.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        public async Task PushModalAsync(Page page)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(page);
            }
        }

        public async Task PopModalAsync()
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }
    }
}