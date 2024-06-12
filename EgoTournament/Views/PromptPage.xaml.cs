namespace EgoTournament.Views;

/// <summary>
/// PromptPage to securize deletes.
/// </summary>
/// <seealso cref="Microsoft.Maui.Controls.ContentPage" />
public partial class PromptPage : ContentPage
{
    /// <summary>
    /// The <see cref="PromptViewModel"/>.
    /// </summary>
    PromptViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptPage" /> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    /// <param name="firebaseService">The firebase service.</param>
    /// <param name="methodType">Type of the method.</param>
    /// <param name="tournamentUidToDelete">The tournament uid to delete.</param>
    public PromptPage(PromptViewModel promptViewModel)
    {
        InitializeComponent();

        BindingContext = promptViewModel;
    }
}