using DevilDaggersSurvivalEditor.Code;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			LabelSurvivalFileLocation.Text = Program.App.userSettings.SurvivalFileLocation;

			Data.DataContext = Program.App.userSettings;

			//foreach (CultureSettings culture in (CultureSettings[])Enum.GetValues(typeof(CultureSettings)))
			//	ComboBoxNumberFormatting.Items.Add(new ComboBoxItem()
			//	{
			//		Content = MiscUtils.ToUserFriendlyString(culture.ToString())
			//	});

			//ComboBoxNumberFormatting.SelectedIndex = (int)Logic.Instance.userSettings.CultureSetting;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = Program.App.userSettings.SurvivalFileLocation
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				Program.App.userSettings.SurvivalFileLocation = dialog.FileName;

			// Make sure this window keeps being focused rather than the MainWindow
			Focus();
		}

		//private void ComboBoxNumberFormatting_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	if (ComboBoxNumberFormatting.SelectedIndex == -1)
		//		return;

		//	Logic.Instance.userSettings.CultureSetting = (CultureSettings)ComboBoxNumberFormatting.SelectedIndex;
		//}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}