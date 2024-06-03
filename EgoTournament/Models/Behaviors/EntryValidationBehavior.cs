using System.Text.RegularExpressions;

namespace EgoTournament.Models.Behaviors
{
    public class EntryValidationBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(EntryValidationBehavior), false);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            // Validar que el texto no sea nulo o vacío
            IsValid = !string.IsNullOrWhiteSpace(entry.Text);

            // Validar que el texto cumpla con ciertas normas (por ejemplo, longitud mínima)
            if (IsValid && entry.Text.Length < 5)
            {
                IsValid = false;
            }

            // Validar que el texto contenga solo letras, números y una sola almohadilla en medio
            if (IsValid && !Regex.IsMatch(entry.Text, @"^[a-zA-Z0-9]+#[a-zA-Z0-9]+$"))
            {
                IsValid = false;
            }

            // Actualizar el color del borde del Entry en función de la validez
            entry.TextColor = IsValid ? Color.FromArgb("#A7AfB2") : Colors.Red;
        }
    }
}