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
			=> TextBlockId.Text = id.ToString();

		public void SetSeconds(double seconds)
			=> TextBlockSeconds.Text = SpawnUtils.ToFramedGameTimeString(seconds + SpawnsetHandler.Instance.Spawnset.TimerStart);

		public void SetDelay(double delay)
			=> TextBlockDelay.Text = SpawnUtils.ToFramedGameTimeString(delay);

		public void SetTotalGems(int totalGems)
			=> TextBlockTotalGems.Text = totalGems.ToString();

		public void SetEnemy(Enemy? enemy, bool gigaBecomesGhost)
		{
			byte spawnsetType = enemy?.SpawnsetType ?? 0xFF;
			Color enemyColor = GuiUtils.EnemyColors.ContainsKey(spawnsetType) ? GuiUtils.EnemyColors[spawnsetType] : GuiUtils.ColorBlack;
			SolidColorBrush background = new(enemyColor);
			SolidColorBrush foreground = ColorUtils.GetPerceivedBrightness(enemyColor) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];

			TextBlockEnemy.Text = enemy?.Name ?? "EMPTY";
			TextBlockEnemy.Background = background;
			TextBlockEnemy.Foreground = foreground;
			TextBlockEnemy.FontWeight = gigaBecomesGhost ? FontWeights.Bold : default;
			TextBlockEnemy.ToolTip = gigaBecomesGhost ? "Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed." : null;
			TextBlockEnemy.TextDecorations = gigaBecomesGhost ? TextDecorations.Underline : null;

			TextBlockNoFarmGems.Text = (enemy?.NoFarmGems ?? 0).ToString();
		}
	}
}
