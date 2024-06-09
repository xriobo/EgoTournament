namespace EgoTournament.Common
{
    public static class Globals
    {
        public const string CURRENT_USER_CREDENTIAL = "currentUserCredential";

        public const string CURRENT_USER = "currentUser";

        public const string ERROR_VALIDATION_TEXT_COLOR = "#c20000";

        public const string OK_VALIDATION_TEXT_COLOR = "#1c2b67";

        public const int MIN_SUMMONERNAME_LENGTH = 5;

        public const int MAX_SUMMONERNAME_LENGTH = 30;

        public static string SUMMONERNAME_VALIDATION_ERROR_MESSAGE = $"The name must contain a hash in the middle of letters and/or numbers. The length must be between {MIN_SUMMONERNAME_LENGTH} and {MAX_SUMMONERNAME_LENGTH}. Example: a7a#mkx7.";
    }
}