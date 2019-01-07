using System.Windows.Controls;

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
			InitializeComponent();

			Grid.DataContext = this;

			ID = id;
			Seconds = seconds;
			EnemyName = enemyName;
			Delay = delay;
			NoFarmGems = noFarmGems;
			TotalGems = totalGems;

			//FontWeight = kvp.Value.IsInLoop ? FontWeights.Bold : FontWeights.Normal
		}
	}
}