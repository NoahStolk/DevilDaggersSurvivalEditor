using DevilDaggersCore.Game;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopSpawnUserControl : UserControl
	{
		private int id;
		private double seconds;
		private string delay;
		private int totalGems;
		private Enemy? enemy;
		private readonly bool gigaBecomesGhost;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		public EndLoopSpawnUserControl(int id, double seconds, string delay, int totalGems, Enemy? enemy, bool gigaBecomesGhost)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			InitializeComponent();

			Id = id;
			Seconds = seconds;
			Delay = delay;
			TotalGems = totalGems;
			Enemy = enemy;
			this.gigaBecomesGhost = gigaBecomesGhost;
		}

		public int Id
		{
			get => id;
			set
			{
				id = value;
				LabelId.Content = value;
			}
		}

		public double Seconds
		{
			get => seconds;
			set
			{
				seconds = value;
				LabelSeconds.Content = value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			}
		}

		public string Delay
		{
			get => delay;
			set
			{
				delay = value;
				LabelDelay.Content = value;
			}
		}

		public int TotalGems
		{
			get => totalGems;
			set
			{
				totalGems = value;
				LabelTotalGems.Content = value;
			}
		}

		public Enemy? Enemy
		{
			get => enemy;
			set
			{
				enemy = value;

				Color enemyColor = enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ColorCode}");
				SolidColorBrush background = new SolidColorBrush(enemyColor);
				SolidColorBrush foreground = new SolidColorBrush(UserInterfaceUtils.GetPerceivedBrightness(enemyColor) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));

				if (gigaBecomesGhost)
				{
					StackPanel stackPanel = new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Background = background,
					};
					stackPanel.Children.Add(new Label
					{
						Content = enemy?.Name ?? "EMPTY",
						Foreground = foreground,
					});
					stackPanel.Children.Add(new Label
					{
						Content = "(?)",
						FontWeight = FontWeights.Bold,
						ToolTip = "Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed.",
					});
					EnemyControl.Content = stackPanel;
				}
				else
				{
					EnemyControl.Content = new Label
					{
						Content = enemy?.Name ?? "EMPTY",
						Background = background,
						Foreground = foreground,
					};
				}

				LabelNoFarmGems.Content = enemy?.NoFarmGems ?? 0;
			}
		}
	}
}