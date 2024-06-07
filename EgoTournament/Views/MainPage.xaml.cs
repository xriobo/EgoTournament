namespace EgoTournament.Views;

public partial class MainPage : ContentPage
{
    /// <summary>
    /// The <see cref="MainViewModel"/>.
    /// </summary>
    private readonly MainViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    /// <param name="mainViewModel">The <see cref="MainViewModel"/>.</param>
    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = mainViewModel;
    }

    /// <summary>
    /// When overridden in a derived class, allows application developers to customize behavior immediately prior to the page becoming visible.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnNavigatedToAsync();
    }
}