using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Web;
using System.Diagnostics;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class UpdateRecommendedWindow : Window
	{
		public UpdateRecommendedWindow()
		{
			InitializeComponent();

			Text.Content = $"{ApplicationUtils.ApplicationDisplayName} {NetworkHandler.Instance.VersionResult.VersionNumberOnline} is available. The current version is {ApplicationUtils.ApplicationVersionNumber}.";
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.ApplicationDownloadUrl(NetworkHandler.Instance.VersionResult.VersionNumberOnline));
			Close();
		}
	}
}