﻿using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Hill : AbstractEllipseArena
	{
		private float startHeight;
		private float endHeight = 8;

		public float StartHeight
		{
			get => startHeight;
			set => startHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float EndHeight
		{
			get => endHeight;
			set => endHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			ArenaCoord center = new ArenaCoord(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY);

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					if (IsPointInEllipse(center.X, center.Y, i, j, InnerRadius, OuterRadius))
					{
						int deltaX = i - center.X;
						int deltaY = j - center.Y;
						float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
						tiles[i, j] = MathUtils.Lerp(startHeight, endHeight, 1 - distance / OuterRadius);
					}

			return tiles;
		}
	}
}