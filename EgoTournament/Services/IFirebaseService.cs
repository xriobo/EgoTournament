using Firebase.Auth;

namespace EgoTournament.Services
{
    public interface IFirebaseService
    {
        Task<bool> SignIn(string email, string password);

        Task<bool> SignUp(string email, string password);
    }
}
