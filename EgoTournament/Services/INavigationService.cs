namespace EgoTournament.Services
{
    public interface INavigationService
    {
        Task PushModalAsync(Page page);

        Task PopModalAsync();
    }
}
