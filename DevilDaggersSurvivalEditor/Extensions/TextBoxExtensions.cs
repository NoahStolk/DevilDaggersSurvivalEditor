using DevilDaggersCore.Wpf.Utils;
using System;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class TextBoxExtensions
	{
		public static bool ValidatePositiveFloatTextBox(this TextBox textBox) => Validate(textBox, (tb) => float.TryParse(tb.Text, out float result) && result >= 0);

		public static bool ValidatePositiveIntTextBox(this TextBox textBox) => Validate(textBox, (tb) => int.TryParse(tb.Text, out int result) && result >= 0);

		public static bool ValidateIntTextBox(this TextBox textBox) => Validate(textBox, (tb) => int.TryParse(tb.Text, out _));

		private static bool Validate(this TextBox textBox, Func<TextBox, bool> validator)
		{
			bool isValid = validator(textBox);
			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];
			return isValid;
		}
	}
}
