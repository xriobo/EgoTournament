using EgoTournament.Models;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    public MainPage(IFirebaseService firebaseService, CurrentUserStore currentUserStore)
    {
        InitializeComponent();

        BindingContext = new LoginViewModel(Navigation, firebaseService, currentUserStore);
    }
}