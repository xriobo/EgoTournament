namespace EgoTournament.Views;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            ((Button)sender).Text = $"Clicked {count} time";
        else
            ((Button)sender).Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(((Button)sender).Text);
    }
}