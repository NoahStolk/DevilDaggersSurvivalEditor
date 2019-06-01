using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public abstract class AbstractArena
	{
		public abstract float[,] GetTiles();

		protected float[,] CreateArenaArray()
		{
			return new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
		}

		protected float[,] VoidArena()
		{
			float[,] tiles = CreateArenaArray();
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					tiles[i, j] = ArenaUtils.VoidDefault;
			return tiles;
		}
	}
}