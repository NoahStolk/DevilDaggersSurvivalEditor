using DevilDaggersSurvivalEditor.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersSurvivalEditor.Windows
{
	public partial class WindowAbout : Window
	{
		public WindowAbout()
		{
			InitializeComponent();

			VersionLabel.Content = $"Version {ApplicationUtils.ApplicationVersionNumber}";
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}