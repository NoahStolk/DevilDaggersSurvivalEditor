using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Full : AbstractArena
	{
		private float _height;

		public float Height
		{
			get => _height;
			set => _height = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override bool IsFull => true;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, Height);

			return tiles;
		}
	}
}
