namespace EgoTournament.ViewModels
{
    /// <summary>
    /// Represents the view model class for data form.
    /// </summary>

    public class LoginViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormViewModel" /> class.
        /// </summary>
        public LoginViewModel()
        {
            this.LoginModel = new LoginModel();
        }

        /// <summary>
        /// Gets or sets the login form model.
        /// </summary>
        public LoginModel LoginModel { get; set; }
    }
}