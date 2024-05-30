namespace EgoTournament.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class TournamentsDetailViewModel : BaseViewModel
{
	[ObservableProperty]
	SampleItem? item;
}
