namespace EgoTournament.Views;

/// <summary>
/// The <see cref="ProfilePage"/>.
/// </summary>
/// <seealso cref="Microsoft.Maui.Controls.ContentPage" />
public partial class ProfilePage : ContentPage
{
    /// <summary>
    /// The <see cref="ProfileViewModel"/>.
    /// </summary>
    ProfileViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilePage"/> class.
    /// </summary>
    /// <param name="viewModel">The <see cref="ProfileViewModel"/>.</param>
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    /// <summary>
    /// When overridden in a derived class, allows application developers to customize behavior immediately prior to the page becoming visible.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnNavigatedToAsync();
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var viewModel = (ProfileViewModel)BindingContext;
        viewModel.CheckBoxChangedCommand.Execute(e.Value);
    }

    private void summonerNameEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = (ProfileViewModel)BindingContext;
        viewModel.SummonerNameEntryTextChangedCommand.Execute(e.NewTextValue);
    }
}