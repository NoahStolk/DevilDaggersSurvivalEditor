using DevilDaggersSurvivalEditor.Code;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSettings : AbstractSpawnsetUserControl
	{
		public SpawnsetSettings()
		{
			InitializeComponent();

			UpdateGUI();

			Data.DataContext = Logic.Instance.spawnset;
		}

		public override void UpdateGUI()
		{
			Dispatcher.Invoke(() =>
			{
				TextBoxShrinkStart.Text = Logic.Instance.spawnset.ShrinkStart.ToString();
				TextBoxShrinkEnd.Text = Logic.Instance.spawnset.ShrinkEnd.ToString();
				TextBoxShrinkRate.Text = Logic.Instance.spawnset.ShrinkRate.ToString();
				TextBoxBrightness.Text = Logic.Instance.spawnset.Brightness.ToString();
			});
		}

		// TODO: Use binding for spawnset arena
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Logic.Instance.MainWindow != null && Logic.Instance.MainWindow.SpawnsetArena != null)
				Logic.Instance.MainWindow.SpawnsetArena.UpdateGUI();
		}
	}
}