using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class HelpWindow : Window
	{
		public HelpWindow()
		{
			InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}