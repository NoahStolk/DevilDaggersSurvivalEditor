using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ModifySpawnDelayWindow : Window
	{
		public ModifySpawnDelayWindow(int spawnCount)
		{
			InitializeComponent();

			SpawnsLabel.Content = $"Modify delay for {spawnCount} spawn{(spawnCount == 1 ? string.Empty : "s")}";

			Data.DataContext = this;

			TextBoxValue.Text = Value.ToString(CultureInfo.InvariantCulture);
			TextBoxValue.Focus();
			TextBoxValue.SelectAll();
		}

		public DelayModificationFunction Function { get; set; }

#pragma warning disable CA1721 // Property names should not match get methods
		public double Value { get; set; } = 2;
#pragma warning restore CA1721 // Property names should not match get methods

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsValueValid())
				DialogResult = true;
		}

		private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool isValid = IsValueValid();

			if (isValid)
				Value = float.Parse(TextBoxValue.Text, CultureInfo.InvariantCulture);

			TextBoxValue.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			OkButton.IsEnabled = isValid;
		}

		private bool IsValueValid()
			=> float.TryParse(TextBoxValue.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;
	}
}