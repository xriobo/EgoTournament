using EgoTournament.Common;
using EgoTournament.Models;
using EgoTournament.Services;

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
    /// Initializes a new instance of the <see cref="PromptPage"/> class.
    /// </summary>
    /// <param name="cacheService">The cache service.</param>
    /// <param name="firebaseService">The firebase service.</param>
    /// <param name="methodType">Type of the method.</param>
    /// <param name="tournaments">The tournaments.</param>
    public PromptPage(ICacheService cacheService, IFirebaseService firebaseService, MethodType methodType, List<TournamentDto> tournaments = null)
    {
        InitializeComponent();

        _viewModel = new PromptViewModel(cacheService, firebaseService, methodType, tournaments);

        BindingContext = _viewModel;
    }
}