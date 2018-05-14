namespace DevilDaggersSurvivalEditor.Models
{
	public class Spawn
	{
		public Enemy enemy;
		public double delay;
		public bool loop;

		public Spawn(Enemy enemy, double delay, bool loop)
		{
			this.enemy = enemy;
			this.delay = delay;
			this.loop = loop;
		}
	}
}