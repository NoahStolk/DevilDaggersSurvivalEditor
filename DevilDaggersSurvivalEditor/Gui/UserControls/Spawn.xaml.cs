using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnUserControl : UserControl
	{
		public SpawnUserControl(Spawn spawn)
		{
			InitializeComponent();

			Spawn = spawn;
			UpdateGui();
		}

		public bool IsInLoop { get; set; }

		public int Id { get; set; }

		public double Seconds { get; set; }

		public int TotalGems { get; set; }

		public Spawn Spawn { get; set; }

		public void UpdateGui()
		{
			FontWeight = IsInLoop ? FontWeights.Bold : FontWeights.Normal;
			Background = new SolidColorBrush(IsInLoop ? Color.FromArgb(127, 127, 63, 63) : Color.FromArgb(0, 0, 0, 0));

			LabelId.Content = Id.ToString(CultureInfo.InvariantCulture);

			LabelSeconds.Content = SpawnUtils.GameTimeToString(Seconds);

			LabelTotalGems.Content = TotalGems.ToString(CultureInfo.InvariantCulture);

			LabelEnemy.Content = Spawn.Enemy?.Name ?? "EMPTY";
			LabelDelay.Content = Spawn.Delay.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			LabelNoFarmGems.Content = Spawn.Enemy?.NoFarmGems ?? 0;

			Color color = Spawn.Enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{Spawn.Enemy.ColorCode}");
			LabelEnemy.Background = new SolidColorBrush(color);
			LabelEnemy.Foreground = ColorUtils.GetPerceivedBrightness(color) < 140 ? ColorUtils.ThemeColors["Text"] : ColorUtils.ThemeColors["Gray1"];
		}
	}
}