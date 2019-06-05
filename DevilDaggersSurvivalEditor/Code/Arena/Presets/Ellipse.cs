﻿using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Ellipse : AbstractEllipseArena
	{
		private float height;

		public float Height
		{
			get => height;
			set => height = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					tiles[i, j] = IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees) ? Height : TileUtils.VoidDefault;

			return tiles;
		}
	}
}