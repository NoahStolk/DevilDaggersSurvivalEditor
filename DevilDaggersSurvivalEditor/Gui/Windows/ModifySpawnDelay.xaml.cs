using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Utils;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows;

public partial class ModifySpawnDelayWindow : Window
{
	public ModifySpawnDelayWindow(int spawnCount)
	{
		InitializeComponent();

		SpawnsLabel.Content = $"Modify delay for {spawnCount} spawn{(spawnCount == 1 ? string.Empty : "s")}";

		Data.DataContext = this;

		TextBoxValue.Text = Value.ToString();
		TextBoxValue.Focus();
		TextBoxValue.SelectAll();
	}

	public DelayModificationFunction Function { get; set; }

	public double Value { get; set; } = 2;

	private void OkButton_Click(object sender, RoutedEventArgs e)
	{
		if (IsValueValid())
			DialogResult = true;
	}

	private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
		=> ValidateValue();

	private void ValidateValue()
	{
		bool isValid = IsValueValid();

		if (isValid)
			Value = float.Parse(TextBoxValue.Text);

		TextBoxValue.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

		OkButton.IsEnabled = isValid;
	}

	private bool IsValueValid()
		=> float.TryParse(TextBoxValue.Text, out float parsed) && parsed < SpawnUtils.MaxDelay && (Function == DelayModificationFunction.Divide ? parsed > 0 : parsed >= 0);

	private void RadioButton_Changed(object sender, RoutedEventArgs e)
		=> ValidateValue();
}
