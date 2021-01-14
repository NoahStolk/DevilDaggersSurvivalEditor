using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Hill : AbstractEllipseArena
	{
		private float _startHeight;
		private float _endHeight = 8;

		public float StartHeight
		{
			get => _startHeight;
			set => _startHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float EndHeight
		{
			get => _endHeight;
			set => _endHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			ArenaCoord center = new ArenaCoord(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY);

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					if (IsPointInEllipse(center.X, center.Y, i, j, InnerRadius, OuterRadius, AngleInDegrees))
					{
						int deltaX = i - center.X;
						int deltaY = j - center.Y;
						float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
						tiles[i, j] = MathUtils.Lerp(_startHeight, _endHeight, 1 - distance / OuterRadius);
					}
				}
			}

			return tiles;
		}
	}
}
