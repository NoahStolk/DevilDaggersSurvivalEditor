namespace DevilDaggersSurvivalEditor.Code.Arena
{
	public struct Coord
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Coord(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override bool Equals(object obj)
		{
			Coord coord = (Coord)obj;

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

		public static bool operator ==(Coord a, Coord b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(Coord a, Coord b)
		{
			return !(a == b);
		}
	}
}