using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code.Network;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class UpdateRecommendedWindow : Window
	{
		public UpdateRecommendedWindow()
		{
			InitializeComponent();

			if (NetworkHandler.Instance.Tool == null)
			{
				App.Log.Warn($"{nameof(UpdateRecommendedWindow)} was opened but tool info was not received.");
				Close();
				return;
			}

			Text.Content = $"{App.ApplicationDisplayName} {NetworkHandler.Instance.Tool.VersionNumber} is available. The current version is {App.LocalVersion}.";
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			ProcessUtils.OpenUrl(UrlUtils.ApiGetTool(App.ApplicationName));
			Close();
		}
	}
}