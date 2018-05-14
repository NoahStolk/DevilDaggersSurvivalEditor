using DevilDaggersSurvivalEditor.Helpers;
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

			VersionLabel.Content = string.Format("Version {0}", Settings.VERSION);
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}