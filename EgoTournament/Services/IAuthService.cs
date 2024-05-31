namespace EgoTournament.Services
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();

        void Login();

        void Logout();
    }
}
