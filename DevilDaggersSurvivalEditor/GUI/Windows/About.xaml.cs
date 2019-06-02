using DevilDaggersSurvivalEditor.Code.Utils;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class AboutWindow : Window
	{
		public AboutWindow()
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