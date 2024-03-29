using DevilDaggersSurvivalEditor.Utils;
using System.Windows;
using System.Windows.Navigation;

namespace DevilDaggersSurvivalEditor.Gui.Windows;

public partial class AboutWindow : Window
{
	public AboutWindow()
	{
		InitializeComponent();

		TextBlockTitle.Text = App.ApplicationDisplayName;
		TextBlockVersion.Text = $"Version {App.LocalVersion}";
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		ProcessUtils.OpenUrl(e.Uri.AbsoluteUri);
		e.Handled = true;
	}
}
