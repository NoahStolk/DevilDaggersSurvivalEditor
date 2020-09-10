using DevilDaggersCore.Game;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopSpawnUserControl : UserControl
	{
		public EndLoopSpawnUserControl(int id, double seconds, string delay, int totalGems, Enemy? enemy, bool gigaBecomesGhost)
		{
			InitializeComponent();

			LabelId.Content = id;
			LabelSeconds.Content = seconds.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			LabelDelay.Content = delay;
			LabelTotalGems.Content = totalGems;

			Color enemyColor = enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ColorCode}");
			SolidColorBrush background = new SolidColorBrush(enemyColor);
			SolidColorBrush foreground = new SolidColorBrush(ColorUtils.GetPerceivedBrightness(enemyColor) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));

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