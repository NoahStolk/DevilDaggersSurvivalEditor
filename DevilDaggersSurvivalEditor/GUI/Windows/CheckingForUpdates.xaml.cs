using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using System.Threading;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public VersionResult VersionResult { get; set; }

		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			Thread thread = new Thread(() =>
			{
				VersionResult versionResult = ApplicationUtils.CheckVersion();
				//Close();
			});
			thread.Start();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			VersionResult = new VersionResult(null, string.Empty, "Cancelled by user");
			Close();
		}
	}
}