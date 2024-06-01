using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class SignUpPage : ContentPage
{
    public SignUpPage(IFirebaseService firebaseService)
	{
		InitializeComponent();

        BindingContext = new SignUpViewModel(Navigation, firebaseService);
    }
}