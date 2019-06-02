using DevilDaggersCore.Spawnset;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Pyramid : AbstractArena
	{
		public float StartHeight { get; set; } = 0;
		public float EndHeight { get; set; } = 6;
		public int Size { get; set; } = 16;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			int halfSize = Size / 2;
			for (int i = 0; i < halfSize; i++)
			{
				int coord = Spawnset.ArenaWidth / 2 - halfSize + i;

				for (int j = 0; j <= (Spawnset.ArenaWidth / 2 - coord) * 2; j++)
				{
					float height = StartHeight + i / (float)halfSize * (EndHeight - StartHeight);

					tiles[coord, coord + j] = height;
					tiles[coord + j, coord] = height;

					tiles[Spawnset.ArenaWidth - 1 - coord, coord + j] = height;
					tiles[coord + j, Spawnset.ArenaHeight - 1 - coord] = height;
				}

				tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] = EndHeight;
			}

			return tiles;
		}
	}
}