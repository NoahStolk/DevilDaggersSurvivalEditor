using DevilDaggersCore.Spawnset;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public abstract class AbstractArena
	{
		public abstract float[,] GetTiles();

		protected float[,] CreateArenaArray()
		{
			return new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
		}

		protected void SetHeightGlobally(float[,] arenaArray, float height)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					arenaArray[i, j] = height;
		}
	}
}