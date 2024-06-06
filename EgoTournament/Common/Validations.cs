using CommunityToolkit.Maui.Alerts;
using EgoTournament.Models.Behaviors;

namespace EgoTournament.Common
{
    public static class Validations
    {
        public static async Task<bool> SummonerName(SummonerNameValidationBehavior entryValidationBehavior, string summonerName, Label validationLabel)
        {
            bool isValid = false;
            if (entryValidationBehavior != null && !entryValidationBehavior.IsValid)
            {
                validationLabel.Text = Globals.SUMMONERNAME_VALIDATION_ERROR_MESSAGE;
                validationLabel.IsVisible = true;
                await Toast.Make($"SummonerName invalid: {summonerName}", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
