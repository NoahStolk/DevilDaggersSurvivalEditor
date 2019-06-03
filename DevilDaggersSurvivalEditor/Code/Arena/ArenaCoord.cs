using DevilDaggersCore.Spawnset;
using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Code.Arena
{
	public struct ArenaCoord
	{
		public const int TileMin = -1;
		public const int TileMax = 63;
		public const int TileDefault = 0;
		public const int VoidDefault = -1000;
		public const int TileSize = 8;
		public const int TileSizeShrunk = 4;

		public int X { get; }
		public int Y { get; }

		public ArenaCoord(int x, int y)
		{
			if (x < 0 || x >= Spawnset.ArenaWidth)
				throw new ArgumentOutOfRangeException("x", $"Parameter {nameof(x)} must be positive and not greater than {Spawnset.ArenaWidth - 1}. {nameof(x)} was {x}.");
			if (y < 0 || y >= Spawnset.ArenaHeight)
				throw new ArgumentOutOfRangeException("y", $"Parameter {nameof(y)} must be positive and not greater than {Spawnset.ArenaHeight - 1}. {nameof(y)} was {y}.");

			X = x;
			Y = y;
		}

		public static Color GetColorFromHeight(float height)
		{
			if (height < TileMin)
				return Color.FromRgb(0, 0, 0);

			float colorValue = Math.Max(0, (height - TileMin) * 12 + 32);

			return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
		}

		public double GetDistanceToCanvasPointSquared(int canvasPoint)
		{
			int canvasX, canvasY;
			if (X > Spawnset.ArenaWidth / 2)
				canvasX = X * TileSize + TileSize;
			else
				canvasX = X * TileSize;

			if (Y > Spawnset.ArenaHeight / 2)
				canvasY = Y * TileSize + TileSize;
			else
				canvasY = Y * TileSize;

			int xFromCenter = canvasX - canvasPoint;
			int yFromCenter = canvasY - canvasPoint;
			return xFromCenter * xFromCenter + (yFromCenter * yFromCenter);
		}

		public override bool Equals(object obj)
		{
			ArenaCoord coord = (ArenaCoord)obj;

			return X == coord.X && Y == coord.Y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				const int HashingBase = (int)2166136261;
				const int HashingMultiplier = 16777619;

				int hash = HashingBase;
				hash = (hash * HashingMultiplier) ^ X.GetHashCode();
				hash = (hash * HashingMultiplier) ^ Y.GetHashCode();
				return hash;
			}
		}

		public static bool operator ==(ArenaCoord a, ArenaCoord b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(ArenaCoord a, ArenaCoord b)
		{
			return !(a == b);
		}
	}
}