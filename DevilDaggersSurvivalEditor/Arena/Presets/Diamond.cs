using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Diamond : AbstractArena
	{
		private int _diamondHalfWidth = 10;
		private int _diamondHalfHeight = 10;
		private float _height;

		public int DiamondHalfWidth
		{
			get => _diamondHalfWidth;
			set => _diamondHalfWidth = Math.Clamp(value, -Spawnset.ArenaWidth / 2, Spawnset.ArenaWidth / 2);
		}

		public int DiamondHalfHeight
		{
			get => _diamondHalfHeight;
			set => _diamondHalfHeight = Math.Clamp(value, -Spawnset.ArenaHeight / 2, Spawnset.ArenaHeight / 2);
		}

		public float Height
		{
			get => _height;
			set => _height = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override bool IsFull => false;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = -_diamondHalfWidth; i <= _diamondHalfWidth; i++)
			{
				for (int j = -_diamondHalfHeight; j <= _diamondHalfHeight; j++)
				{
					int sum = Math.Abs(i) + Math.Abs(j);
					tiles[i + Spawnset.ArenaWidth / 2, j + Spawnset.ArenaHeight / 2] = sum > Math.Max(_diamondHalfWidth, _diamondHalfHeight) ? TileUtils.VoidDefault : Height;
				}
			}

			return tiles;
		}
	}
}
