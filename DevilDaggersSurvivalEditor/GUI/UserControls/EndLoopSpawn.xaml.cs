using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class EndLoopSpawnControl : UserControl
	{
		private int id;
		public int ID
		{
			get => id;
			set
			{
				id = value;
				LabelID.Content = value;
			}
		}

		private double seconds;
		public double Seconds
		{
			get => seconds;
			set
			{
				seconds = value;
				LabelSeconds.Content = value.ToString("0.00");
			}
		}

		private string delay;
		public string Delay
		{
			get => delay;
			set
			{
				delay = value;
				LabelDelay.Content = value;
			}
		}

		private int totalGems;
		public int TotalGems
		{
			get => totalGems;
			set
			{
				totalGems = value;
				LabelTotalGems.Content = value;
			}
		}

		private SpawnsetEnemy enemy;
		public SpawnsetEnemy Enemy
		{
			get => enemy;
			set
			{
				enemy = value;

				LabelEnemy.Content = enemy.Name;
				LabelNoFarmGems.Content = enemy.NoFarmGems;

				Color color = enemy == Spawnset.Enemies[-1] ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ToEnemy(GameInfo.GameVersions[GameInfo.DEFAULT_GAME_VERSION]).ColorCode}");
				LabelEnemy.Background = new SolidColorBrush(color);

				if (MiscUtils.GetPerceivedBrightness(color) < 140)
					LabelEnemy.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			}
		}

		public EndLoopSpawnControl()
		{
			InitializeComponent();

			Background = new SolidColorBrush(Color.FromArgb(192, 255, 255, 255));
		}

		public void AddGigaGhostToolTip()
		{
			LabelEnemy.ToolTip = "Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed.";
		}
	}
}