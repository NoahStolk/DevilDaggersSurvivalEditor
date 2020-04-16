using DevilDaggersSurvivalEditor.Code.Spawns;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ModifySpawnDelayWindow : Window
	{
		internal DelayModificationFunction Function { get; set; }
		public double Value { get; set; } = 2;

		public ModifySpawnDelayWindow(int spawnCount)
		{
			InitializeComponent();

			SpawnsLabel.Content = $"Modify delay for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			Data.DataContext = this;

			TextBoxValue.Text = Value.ToString();
			TextBoxValue.Focus();
			TextBoxValue.SelectAll();

			foreach (DelayModificationFunction dmf in (DelayModificationFunction[])Enum.GetValues(typeof(DelayModificationFunction)))
				FunctionComboBox.Items.Add(new ComboBoxItem { Content = dmf.ToString() });
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			FunctionComboBox.SelectedIndex = 0;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsValueValid())
				DialogResult = true;
		}

		private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool valid = IsValueValid();

			if (valid)
				Value = float.Parse(TextBoxValue.Text);

			TextBoxValue.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 128, 128));

			OkButton.IsEnabled = valid;
		}

		private bool IsValueValid()
		{
			return float.TryParse(TextBoxValue.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;
		}
	}
}