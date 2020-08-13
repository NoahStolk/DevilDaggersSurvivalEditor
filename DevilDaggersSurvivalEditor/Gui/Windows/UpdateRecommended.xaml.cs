using DevilDaggersCore.Tools;
using DevilDaggersCore.Utils;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class UpdateRecommendedWindow : Window
	{
		public UpdateRecommendedWindow()
		{
			InitializeComponent();

			Text.Content = $"{App.ApplicationDisplayName} {VersionHandler.Instance.VersionResult.Tool.VersionNumber} is available. The current version is {App.LocalVersion}.";
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			ProcessUtils.OpenUrl(UrlUtils.ApiGetTool(App.ApplicationName));
			Close();
		}
	}
}