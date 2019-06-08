using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnControl : UserControl
	{
		public int ID { get; set; }
		public double Seconds { get; set; }
		public string EnemyName { get; set; }
		public double Delay { get; set; }
		public int NoFarmGems { get; set; }
		public int TotalGems { get; set; }

		public SpawnControl(int id, double seconds, string enemyName, double delay, int noFarmGems, int totalGems)
		{
			ID = id;
			Seconds = seconds;
			EnemyName = enemyName;
			Delay = delay;
			NoFarmGems = noFarmGems;
			TotalGems = totalGems;

			InitializeComponent();

			Grid.DataContext = this;

			SpawnsetEnemy enemy = Spawnset.Enemies.Where(k => k.Value.Name == enemyName).FirstOrDefault().Value;
			Color color = enemy == Spawnset.Enemies[-1] ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{enemy.ToEnemy(Game.GameVersions[Game.DEFAULT_GAME_VERSION]).ColorCode}");
			Enemy.Background = new SolidColorBrush(color);

			if (MiscUtils.GetPerceivedBrightness(color) < 140)
				Enemy.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
		}
	}
}