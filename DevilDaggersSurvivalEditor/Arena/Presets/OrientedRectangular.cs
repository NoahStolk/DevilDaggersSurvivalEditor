using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Numerics;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class OrientedRectangular : AbstractArena
	{
		private float _height;
		private float _ax = 5;
		private float _ay = 5;
		private float _bx = 15;
		private float _by = 20;
		private float _cx = 8;
		private float _cy = 25;

		public float Height
		{
			get => _height;
			set => _height = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float AX
		{
			get => _ax;
			set => _ax = Math.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}

		public float AY
		{
			get => _ay;
			set => _ay = Math.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}

		public float BX
		{
			get => _bx;
			set => _bx = Math.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}

		public float BY
		{
			get => _by;
			set => _by = Math.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}

		public float CX
		{
			get => _cx;
			set => _cx = Math.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}

		public float CY
		{
			get => _cy;
			set => _cy = Math.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}

		public override bool IsFull => false;

		public bool IsPointInRectangle(Vector2 point)
		{
			Vector2 a = new Vector2(_ax, _ay);
			Vector2 b = new Vector2(_bx, _by);
			Vector2 c = new Vector2(_cx, _cy);

			Vector2 ab = Vector(a, b);
			Vector2 am = Vector(a, point);
			Vector2 bc = Vector(b, c);
			Vector2 bm = Vector(b, point);
			float dotABAM = Dot(ab, am);
			float dotABAB = Dot(ab, ab);
			float dotBCBM = Dot(bc, bm);
			float dotBCBC = Dot(bc, bc);
			return dotABAM >= 0 && dotABAM <= dotABAB && dotBCBM >= 0 && dotBCBM <= dotBCBC;
		}

		public static Vector2 Vector(Vector2 p1, Vector2 p2)
			=> new Vector2(p2.X - p1.X, p2.Y - p1.Y);

		public static float Dot(Vector2 u, Vector2 v)
			=> u.X * v.X + u.Y * v.Y;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					if (IsPointInRectangle(new Vector2(i, j)))
						tiles[i, j] = Height;
				}
			}

			return tiles;
		}
	}
}