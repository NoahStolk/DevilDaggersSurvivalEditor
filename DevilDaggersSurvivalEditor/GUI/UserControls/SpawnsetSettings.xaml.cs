using DevilDaggersSurvivalEditor.Code;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSettings : AbstractSpawnsetUserControl
	{
		public SpawnsetSettings()
		{
			InitializeComponent();
		}

		public override void UpdateGUI()
		{
			Dispatcher.Invoke(() =>
			{
				ShrinkStart.Text = Logic.Instance.spawnset.ShrinkStart.ToString("0.####");
				ShrinkEnd.Text = Logic.Instance.spawnset.ShrinkEnd.ToString("0.####");
				ShrinkRate.Text = Logic.Instance.spawnset.ShrinkRate.ToString("0.####");
				Brightness.Text = Logic.Instance.spawnset.Brightness.ToString("0.####");
			});
		}
	}
}