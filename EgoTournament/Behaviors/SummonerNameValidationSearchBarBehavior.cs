using EgoTournament.Common;
using System.Text.RegularExpressions;

namespace EgoTournament.Behaviors
{
    public class SummonerNameValidationSearchBarBehavior : Behavior<SearchBar>
    {
        public static readonly BindableProperty IsValidProperty =
                BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(SummonerNameValidationSearchBarBehavior), false);

        private const string RegExPattern = @"^[a-zA-Z0-9]+#[a-zA-Z0-9]+$";

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        protected override void OnAttachedTo(SearchBar bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as SearchBar;
            IsValid = !string.IsNullOrWhiteSpace(entry.Text);

            if (IsValid && (entry.Text.Length < Globals.MIN_SUMMONERNAME_LENGTH || entry.Text.Length > Globals.MAX_SUMMONERNAME_LENGTH))
            {
                IsValid = false;
            }

            if (IsValid && !Regex.IsMatch(entry.Text, RegExPattern))
            {
                IsValid = false;
            }

            entry.TextColor = IsValid ? Color.FromArgb(Globals.OK_VALIDATION_TEXT_COLOR) : Color.FromArgb(Globals.ERROR_VALIDATION_TEXT_COLOR);
        }
    }
}