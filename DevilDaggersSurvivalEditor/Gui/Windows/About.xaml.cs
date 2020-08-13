using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();

			TitleLabel.Content = App.ApplicationDisplayName;
			VersionLabel.Content = $"Version {App.LocalVersion}";
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
			e.Handled = true;
		}
	}
}