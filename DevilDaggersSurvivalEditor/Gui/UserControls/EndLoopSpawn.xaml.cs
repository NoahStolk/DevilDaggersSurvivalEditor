using DevilDaggersCore.Game;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopSpawnUserControl : UserControl
	{
		public EndLoopSpawnUserControl()
			=> InitializeComponent();

		public void SetId(int id)
			=> LabelId.Content = id;

		public void SetSeconds(double seconds)
			=> LabelSeconds.Content = SpawnUtils.ToFramedGameTimeString(seconds + SpawnsetHandler.Instance.Spawnset.TimerStart);

		public void SetDelay(double delay)
			=> LabelDelay.Content = SpawnUtils.ToFramedGameTimeString(delay);

		public void SetTotalGems(int totalGems)
			=> LabelTotalGems.Content = totalGems;

		public void SetEnemy(Enemy? enemy, bool gigaBecomesGhost)
		{
			Color enemyColor = enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ColorCode}");
			SolidColorBrush background = new(enemyColor);
			SolidColorBrush foreground = ColorUtils.GetPerceivedBrightness(enemyColor) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];

			if (gigaBecomesGhost)
			{
				StackPanel stackPanel = new()
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
