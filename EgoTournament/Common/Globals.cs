namespace EgoTournament.Common
{
    public static class Globals
    {
        public const string CURRENT_USER_CREDENTIAL = "currentUserCredential";

        public const string CURRENT_USER = "currentUser";

        public const string ERROR_VALIDATION_TEXT_COLOR = "#d52929";

        public const string OK_VALIDATION_TEXT_COLOR = "#1c2b67";

        public const int MIN_SUMMONERNAME_LENGTH = 5;

        public const int MAX_SUMMONERNAME_LENGTH = 30;

        public static string SUMMONERNAME_VALIDATION_ERROR_MESSAGE = $"SummonerName contains hash symbol in middle, letters and numbers. Length {MIN_SUMMONERNAME_LENGTH} to {MAX_SUMMONERNAME_LENGTH}. Sample: a12#a2w";
    }
}