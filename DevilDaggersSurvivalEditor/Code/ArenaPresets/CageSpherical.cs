using DevilDaggersCore.Spawnset;
using System;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class CageSpherical : AbstractArena
	{
		public float InsideHeight { get; set; }
		public float WallHeight { get; set; } = 8;
		public int Radius { get; set; } = 8;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = Spawnset.ArenaWidth / 2 - Radius; i <= Spawnset.ArenaWidth / 2 + Radius; i++)
				for (int j = Spawnset.ArenaHeight / 2 - Radius; j <= Spawnset.ArenaHeight / 2 + Radius; j++)
					tiles[i, j] = Math.Sqrt(Math.Pow(i - Spawnset.ArenaWidth / 2, 2) + Math.Pow(j - Spawnset.ArenaHeight / 2, 2)) < Radius ? InsideHeight : WallHeight;

			return tiles;
		}
	}
}