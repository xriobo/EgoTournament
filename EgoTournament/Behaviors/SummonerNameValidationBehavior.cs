using CommunityToolkit.Maui.Alerts;
using EgoTournament.Common;
using System.Text.RegularExpressions;

namespace EgoTournament.Behaviors
{
    public class SummonerNameValidationBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
                BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(SummonerNameValidationBehavior), false);

        private const string RegExPattern = @"^[a-zA-Z0-9]+#[a-zA-Z0-9]+$";

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        async void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
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