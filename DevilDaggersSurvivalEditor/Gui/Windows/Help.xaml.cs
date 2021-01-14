using DevilDaggersCore.Utils;
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
			ProcessUtils.OpenUrl(e.Uri.AbsoluteUri);
			e.Handled = true;
		}
	}
}
