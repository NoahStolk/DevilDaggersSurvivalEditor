using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public abstract class AbstractArena
	{
		public abstract float[,] GetTiles();

		protected float[,] VoidArena()
		{
			float[,] tiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					tiles[i, j] = ArenaUtils.VoidDefault;
			return tiles;
		}
	}
}