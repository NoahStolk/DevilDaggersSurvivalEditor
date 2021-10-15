using DevilDaggersCore.Spawnsets;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class CageEllipse : AbstractEllipseArena
	{
		private int _wallThickness = 1;

		public float InsideHeight { get; set; }

		public float WallHeight { get; set; } = 8;

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
