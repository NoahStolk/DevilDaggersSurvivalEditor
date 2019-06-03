using DevilDaggersCore.Spawnset;
using NetBase.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Qbert : AbstractRectangularArena
	{
		private int offsetX;
		private int offsetY;
		private float startheight = -1;
		private float endHeight = 17;

		public int OffsetX
		{
			get => offsetX;
			set => offsetX = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth);
		}
		public int OffsetY
		{
			get => offsetY;
			set => offsetY = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight);
		}
		public float StartHeight
		{
			get => startheight;
			set => startheight = MathUtils.Clamp(value, ArenaUtils.TileMin, ArenaUtils.TileMax);
		}
		public float EndHeight
		{
			get => endHeight;
			set => endHeight = MathUtils.Clamp(value, ArenaUtils.TileMin, ArenaUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

			float stepX = (StartHeight - EndHeight) / (X2 - X1 - 1);
			float stepY = (StartHeight - EndHeight) / (Y2 - Y1 - 1);
			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i + OffsetX, j + OffsetY] = EndHeight + (Math.Abs(i - Spawnset.ArenaWidth / 2) * stepX + Math.Abs(j - Spawnset.ArenaHeight / 2) * stepY);

			return tiles;
		}
	}
}