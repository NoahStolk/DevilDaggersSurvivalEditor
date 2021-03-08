using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System.Globalization;
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
			LabelId.Content = id.ToString(CultureInfo.InvariantCulture);
		}

		public void SetSeconds(double seconds)
		{
			Seconds = seconds;
			LabelSeconds.Content = SpawnUtils.ToFramedGameTimeString(seconds + SpawnsetHandler.Instance.Spawnset.TimerStart);
		}

		public void SetTotalGems(int totalGems)
		{
			TotalGems = totalGems;
			LabelTotalGems.Content = totalGems.ToString(CultureInfo.InvariantCulture);
		}

		public void SetSpawn(Spawn spawn)
		{
			Spawn = spawn;

			LabelEnemy.Content = spawn.Enemy?.Name ?? "EMPTY";
			LabelDelay.Content = spawn.Delay.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			LabelNoFarmGems.Content = spawn.Enemy?.NoFarmGems ?? 0;

			Color color = spawn.Enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{spawn.Enemy.ColorCode}");
			LabelEnemy.Background = new SolidColorBrush(color);
			LabelEnemy.Foreground = ColorUtils.GetPerceivedBrightness(color) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];
		}

		public void SetIsInLoop(bool isInLoop)
		{
			IsInLoop = isInLoop;

			FontWeight = isInLoop ? FontWeights.Bold : FontWeights.Normal;
			Background = new SolidColorBrush(isInLoop ? Color.FromArgb(127, 127, 63, 63) : Color.FromArgb(0, 0, 0, 0));
		}
	}
}
