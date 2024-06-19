namespace EgoTournament.Common
{
    public static class Globals
    {
        public const string CURRENT_USER_CREDENTIAL = "currentUserCredential";

        public const string CURRENT_USER = "currentUser";

        public const string ERROR_VALIDATION_TEXT_COLOR = "#c20000";

        public const string OK_VALIDATION_TEXT_COLOR = "#1c2b67";

        public const int MIN_SUMMONERNAME_LENGTH = 7;

        public const int MAX_SUMMONERNAME_LENGTH = 21;

        public static string SUMMONERNAME_VALIDATION_ERROR_MESSAGE = $"It must be an existing RiotId. It must have a # in the middle. Spaces are allowed between letters and/or numbers in the first half, but not in the second. The length must be between {MIN_SUMMONERNAME_LENGTH} - {MAX_SUMMONERNAME_LENGTH}. Example: A7Z#MK7E.";

        /// <summary>
        /// The reg ex pattern.
        /// La longitud total debe estar entre 7 y 21 caracteres.
        /// Debe haber entre 3 y 5 caracteres alfanuméricos después del #.
        /// Permitir espacios en blanco antes y después del #, pero no después del #.
        /// </summary>
        public const string SUMMONERNAME_REGEX_PATTERN = @"^(?=[a-zA-Z0-9 ]{3,16}#[a-zA-Z0-9]{3,5}$)[a-zA-Z0-9 ]{3,16}#[a-zA-Z0-9]{3,5}$";

        public const int SECONDS_TO_REFRESH = 10;

        public const string UPGRADE_ACCOUNT_MESSAGE = "Enjoy exclusive features by upgrading to a Admin or Premium account. Would you like to upgrade now?";

        public const string UPGRADE_PREMIUM_MESSAGE = "Enjoy exclusive features by upgrading to a Premium account. Would you like to upgrade now?";

        public const string WINDOW_PAYMENTS_MESSAGE = "Window for managing account upgrades and payments.";

        public const string UPGRADE_PREMIUM_TITLE = "UPGRADE TO PREMIUM";

        public const string UPGRADE_ACCOUNT_TITLE = "UPGRADE ACCOUNT";

        public const string MANAGE_PAYMENT_TITLE = "MANAGE PAYMENTS";

        public const string NOT_OWNER_TITLE = "NOT OWNER";

        public const string CHAMPION_POSITION = "Champion";

        public const string SECOND_POSITION = "Second";

        public const string THIRD_POSITION = "Third";

        public const string CURRENCY = "EUR";
    }
}