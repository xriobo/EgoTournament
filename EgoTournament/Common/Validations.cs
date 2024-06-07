using CommunityToolkit.Maui.Alerts;
using System.Text.RegularExpressions;

namespace EgoTournament.Common
{
    public static class Validations
    {
        private const string RegExPattern = @"^[a-zA-Z0-9]+#[a-zA-Z0-9]+$";

        public static bool SummonerName(string summonerName)
        {
            bool isValid = true;
            if (isValid && (summonerName.Length < Globals.MIN_SUMMONERNAME_LENGTH || summonerName.Length > Globals.MAX_SUMMONERNAME_LENGTH))
            {
                isValid = false;
            }

            if (isValid && !Regex.IsMatch(summonerName, RegExPattern))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
