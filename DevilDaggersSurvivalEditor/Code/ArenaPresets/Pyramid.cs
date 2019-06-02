using DevilDaggersCore.Spawnset;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Pyramid : AbstractArena
	{
		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
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

					tiles[coord + OffsetX, coord + j + OffsetY] = height;
					tiles[coord + j + OffsetX, coord + OffsetY] = height;

					tiles[Spawnset.ArenaWidth - 1 - coord + OffsetX, coord + j + OffsetY] = height;
					tiles[coord + j + OffsetX, Spawnset.ArenaHeight - 1 - coord + OffsetY] = height;
				}

				tiles[Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY] = EndHeight;
			}

			return tiles;
		}
	}
}