using DevilDaggersCore.Wpf.Utils;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Extensions
{
	public static class TextBoxExtensions
	{
		public static bool ValidatePositiveFloatTextBox(this TextBox textBox)
		{
			bool isValid = float.TryParse(textBox.Text, out float result) && result >= 0;
			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];
			return isValid;
		}

		public static bool ValidatePositiveIntTextBox(this TextBox textBox)
		{
			bool isValid = int.TryParse(textBox.Text, out int result) && result >= 0;
			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];
			return isValid;
		}
	}
}
