using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnUserControl : UserControl
	{
		public SpawnUserControl()
			=> InitializeComponent();

		public int Id { get; private set; }
		public double Seconds { get; private set; }
		public int TotalGems { get; private set; }
		public Spawn? Spawn { get; private set; }
		public bool IsInLoop { get; private set; }

		public void SetId(int id)
		{
			Id = id;
			TextBlockId.Text = id.ToString();
		}

		public void SetSeconds(double seconds)
		{
			Seconds = seconds;
			TextBlockSeconds.Text = SpawnUtils.ToFramedGameTimeString(seconds + SpawnsetHandler.Instance.Spawnset.TimerStart);
		}

		public void SetTotalGems(int totalGems)
		{
			TotalGems = totalGems;
			TextBlockTotalGems.Text = totalGems.ToString();
		}

		public void SetSpawn(Spawn spawn)
		{
			Spawn = spawn;

			TextBlockEnemy.Text = spawn.Enemy?.Name ?? "EMPTY";
			TextBlockDelay.Text = spawn.Delay.ToString(SpawnUtils.Format);
			TextBlockNoFarmGems.Text = (spawn.Enemy?.NoFarmGems ?? 0).ToString();

			Color color = spawn.Enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{spawn.Enemy.ColorCode}");
			TextBlockEnemy.Background = new SolidColorBrush(color);
			TextBlockEnemy.Foreground = ColorUtils.GetPerceivedBrightness(color) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];
		}

		public void SetIsInLoop(bool isInLoop)
		{
			IsInLoop = isInLoop;

			FontWeight = isInLoop ? FontWeights.Bold : FontWeights.Normal;
			Background = new SolidColorBrush(isInLoop ? Color.FromArgb(127, 127, 63, 63) : Color.FromArgb(0, 0, 0, 0));
		}
	}
}
