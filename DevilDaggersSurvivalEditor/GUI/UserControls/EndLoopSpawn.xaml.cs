using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class EndLoopSpawnUserControl : UserControl
	{
		public bool GigaBecomesGhost { get; set; }

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
				LabelSeconds.Content = value.ToString(SpawnUtils.Format);
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

				Color enemyColor = enemy == Spawnset.Enemies[-1] ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ToEnemy(GameInfo.GameVersions[GameInfo.DEFAULT_GAME_VERSION]).ColorCode}");
				SolidColorBrush background = new SolidColorBrush(enemyColor);
				SolidColorBrush foreground = new SolidColorBrush(MiscUtils.GetPerceivedBrightness(enemyColor) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));

				if (GigaBecomesGhost)
				{
					StackPanel stackPanel = new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Background = background
					};
					stackPanel.Children.Add(new Label
					{
						Content = enemy.Name,
						Foreground = foreground
					});
					stackPanel.Children.Add(new Label
					{
						Content = "(?)",
						FontWeight = FontWeights.Bold,
						ToolTip = "Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed."
					});
					EnemyControl.Content = stackPanel;
				}
				else
				{
					EnemyControl.Content = new Label
					{
						Content = enemy.Name,
						Background = background,
						Foreground = foreground
					};
				}

				LabelNoFarmGems.Content = enemy.NoFarmGems;
			}
		}

		public EndLoopSpawnUserControl()
		{
			InitializeComponent();
		}
	}
}