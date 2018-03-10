using DevilDaggersSpawnsetEditorWPF.Helpers;
using System.Collections.Generic;

namespace DevilDaggersSpawnsetEditorWPF.Models
{
	public class Spawnset
	{
		public Dictionary<int, Spawn> spawns;
		public float[,] arenaTiles;
		public float shrinkStart;
		public float shrinkEnd;
		public float shrinkRate;
		public float brightness;

		public Spawnset()
		{
			spawns = new Dictionary<int, Spawn>();
			arenaTiles = new float[SpawnsetParser.ARENA_WIDTH, SpawnsetParser.ARENA_HEIGHT];
			shrinkStart = 50;
			shrinkEnd = 20;
			shrinkRate = 0.025f;
			brightness = 60;
		}

		public Spawnset(Dictionary<int, Spawn> spawns, float[,] arenaTiles, float shrinkStart, float shrinkEnd, float shrinkRate, float brightness)
		{
			this.spawns = spawns;
			this.arenaTiles = arenaTiles;
			this.shrinkStart = shrinkStart;
			this.shrinkEnd = shrinkEnd;
			this.shrinkRate = shrinkRate;
			this.brightness = brightness;
		}
	}
}