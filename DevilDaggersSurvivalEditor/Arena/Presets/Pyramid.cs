using DevilDaggersCore.Spawnsets;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Pyramid : AbstractArena
	{
		private int _offsetX;
		private int _offsetY;
		private float _startheight;
		private float _endHeight = 6;
		private int _size = 16;

		public int OffsetX
		{
			get => _offsetX;
			set => _offsetX = Math.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}

		public int OffsetY
		{
			get => _offsetY;
			set => _offsetY = Math.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}

		public float StartHeight
		{
			get => _startheight;
			set => _startheight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float EndHeight
		{
			get => _endHeight;
			set => _endHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public int Size
		{
			get => _size;
			set => _size = Math.Clamp(value, 2, Spawnset.ArenaWidth);
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