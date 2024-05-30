namespace EgoTournament.ExternalServices
{
    /// <summary>
    /// Interface Firebase Authentication Service.
    /// </summary>
    public interface IFirebaseAuthService
    {
        /// <summary>
        /// Logins the with email password.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <returns>A string.</returns>
        Task<string> LoginWithEmailPassword(string email, string password);
    }
}
