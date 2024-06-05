﻿using CommunityToolkit.Maui.Alerts;
using EgoTournament.Services;

namespace EgoTournament.Views;

public partial class MainPage : ContentPage
{
    private readonly ICacheService _cacheService;

    public MainPage(ICacheService cacheService)
    {
        InitializeComponent();
        this._cacheService = cacheService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _cacheService.GetCurrentUserCredentialAsync() != null)
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            await Toast.Make("You must Sign In.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
    }
}