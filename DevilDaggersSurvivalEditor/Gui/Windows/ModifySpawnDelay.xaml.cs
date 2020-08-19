using DevilDaggersSurvivalEditor.Code.Spawns;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

			foreach (DelayModificationFunction dmf in (DelayModificationFunction[])Enum.GetValues(typeof(DelayModificationFunction)))
				FunctionComboBox.Items.Add(new ComboBoxItem { Content = dmf.ToString() });
		}

		public DelayModificationFunction Function { get; set; }

#pragma warning disable CA1721 // Property names should not match get methods
		public double Value { get; set; } = 2;
#pragma warning restore CA1721 // Property names should not match get methods

		private void Window_Loaded(object sender, RoutedEventArgs e)
			=> FunctionComboBox.SelectedIndex = 0;

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsValueValid())
				DialogResult = true;
		}

		private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool valid = IsValueValid();

			if (valid)
				Value = float.Parse(TextBoxValue.Text, CultureInfo.InvariantCulture);

			TextBoxValue.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));

			OkButton.IsEnabled = valid;
		}

		private bool IsValueValid()
			=> float.TryParse(TextBoxValue.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;
	}
}