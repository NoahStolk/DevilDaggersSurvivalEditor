using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public abstract class AbstractSpawnsetUserControl : UserControl
	{
		protected AbstractSpawnsetUserControl()
		{
		}

		// TODO: Replace with SpawnsetLoaded method
		public abstract void UpdateGUI();
	}
}