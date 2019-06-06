using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena
{
	/// <summary>
	/// Provides a simple structure containing a read-only integer coordinate that is specific to the arena.
	/// This struct should not be used for arena indexing (hence why there is no tile height property), as coordinates are useless in that case (indexing is done through a two-dimensional array of floats).
	/// Use the struct for GUI-related tasks only.
	/// </summary>
	public struct ArenaCoord
	{
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

		public double GetDistanceToCanvasPointSquared(int canvasPoint)
		{
			int canvasX, canvasY;
			if (X > Spawnset.ArenaWidth / 2)
				canvasX = X * TileUtils.TileSize + TileUtils.TileSize;
			else
				canvasX = X * TileUtils.TileSize;

			if (Y > Spawnset.ArenaHeight / 2)
				canvasY = Y * TileUtils.TileSize + TileUtils.TileSize;
			else
				canvasY = Y * TileUtils.TileSize;

			int xFromCenter = canvasX - canvasPoint;
			int yFromCenter = canvasY - canvasPoint;
			return xFromCenter * xFromCenter + yFromCenter * yFromCenter;
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