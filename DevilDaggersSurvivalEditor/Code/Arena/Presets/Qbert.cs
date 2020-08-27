using DevilDaggersCore.Spawnsets;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Qbert : AbstractRectangularArena
	{
		private int _offsetX;
		private int _offsetY;
		private float _startheight = -1;
		private float _endHeight = 17;

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

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			float stepX = (StartHeight - EndHeight) / (X2 - X1 - 1);
			float stepY = (StartHeight - EndHeight) / (Y2 - Y1 - 1);
			for (int i = X1; i < X2; i++)
			{
				for (int j = Y1; j < Y2; j++)
					tiles[i + OffsetX, j + OffsetY] = EndHeight + (Math.Abs(i - Spawnset.ArenaWidth / 2) * stepX + Math.Abs(j - Spawnset.ArenaHeight / 2) * stepY);
			}

			return tiles;
		}
	}
}