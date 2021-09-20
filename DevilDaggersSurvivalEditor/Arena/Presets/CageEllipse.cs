using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class CageEllipse : AbstractEllipseArena
	{
		private float _insideHeight;
		private float _wallHeight = 8;
		private int _wallThickness = 1;

		public float InsideHeight
		{
			get => _insideHeight;
			set => _insideHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float WallHeight
		{
			get => _wallHeight;
			set => _wallHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public int WallThickness
		{
			get => _wallThickness;
			set => _wallThickness = Math.Clamp(value, 1, 20);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
						tiles[i, j] = InsideHeight;
					else if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius + WallThickness, OuterRadius + WallThickness, AngleInDegrees))
						tiles[i, j] = WallHeight;
				}
			}

			return tiles;
		}
	}
}
