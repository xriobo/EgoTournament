using System.Text.RegularExpressions;

namespace EgoTournament.Common
{
    /// <summary>
    /// Validations class.
    /// </summary>
    public static class Validations
    {
        /// <summary>
        /// Summoners the name.
        /// </summary>
        /// <param name="summonerName">Name of the summoner.</param>
        /// <returns>True if is valid summoner name otherwise false.</returns>
        public static bool SummonerName(string summonerName)
        {
            bool isValid = true;
            if (isValid && summonerName.Length < Globals.MIN_SUMMONERNAME_LENGTH && summonerName.Length > Globals.MAX_SUMMONERNAME_LENGTH)
            {
                isValid = false;
            }

            if (isValid && !Regex.IsMatch(summonerName, Globals.SUMMONERNAME_REGEX_PATTERN))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
