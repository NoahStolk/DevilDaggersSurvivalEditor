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

		// TODO: Remove
		public override void UpdateGUI()
		{
		}

		// TODO: Use binding for spawnset arena
		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Logic.Instance.MainWindow != null && Logic.Instance.MainWindow.SpawnsetArena != null)
				Logic.Instance.MainWindow.SpawnsetArena.UpdateGUI();
		}
	}
}