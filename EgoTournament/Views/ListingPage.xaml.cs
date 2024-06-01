using Newtonsoft.Json;
using Firebase.Auth;
using EgoTournament.Models;

namespace EgoTournament.Views;

public partial class ListingPage : ContentPage
{
    private readonly CurrentUserStore _currentUserStore;

    public ListingPage(CurrentUserStore currentUserStore)
    {
        this._currentUserStore = currentUserStore;
        InitializeComponent();
        GetProfileInfo();
    }

    private void GetProfileInfo()
    {
        UserEmail.Text = _currentUserStore.CurrentUser.Info.Email;
    }
}