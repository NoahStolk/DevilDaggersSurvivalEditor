using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Pyramid : AbstractArena
	{
		private int offsetX;
		private int offsetY;
		private float startheight;
		private float endHeight = 6;
		private int size = 16;

		public int OffsetX
		{
			get => offsetX;
			set => offsetX = MathUtils.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}

		public int OffsetY
		{
			get => offsetY;
			set => offsetY = MathUtils.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}

		public float StartHeight
		{
			get => startheight;
			set => startheight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float EndHeight
		{
			get => endHeight;
			set => endHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public int Size
		{
			get => size;
			set => size = MathUtils.Clamp(value, 2, Spawnset.ArenaWidth);
		}

		public override bool IsFull => false;

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