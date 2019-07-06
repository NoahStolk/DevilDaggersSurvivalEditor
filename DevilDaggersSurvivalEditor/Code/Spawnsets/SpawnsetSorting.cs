using DevilDaggersCore.Spawnset.Web;
using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public class SpawnsetSorting
	{
		public string Name { get; }
		public Func<SpawnsetFile, object> SortingFunction { get; }
		public bool IsAscendingDefault { get; }

		public bool Ascending { get; set; }

		public SpawnsetSorting(string name, Func<SpawnsetFile, object> sortingFunction, bool isAscendingDefault)
		{
			Name = name;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}
	}
}