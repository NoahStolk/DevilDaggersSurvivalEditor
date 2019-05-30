using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public abstract class AbstractUserControl : UserControl
	{
		protected Spawnset spawnset;
		protected UserSettings userSettings;

		protected AbstractUserControl()
		{
			spawnset = Logic.Instance.spawnset;
			userSettings = Logic.Instance.userSettings;
		}
	}
}