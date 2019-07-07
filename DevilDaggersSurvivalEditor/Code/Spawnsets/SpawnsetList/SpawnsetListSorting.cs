using DevilDaggersCore.Spawnset.Web;
using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class SpawnsetListSorting
	{
		public string Name { get; }
		public Func<SpawnsetFile, object> SortingFunction { get; }
		public bool IsAscendingDefault { get; }

		public bool Ascending { get; set; }

		public SpawnsetListSorting(string name, Func<SpawnsetFile, object> sortingFunction, bool isAscendingDefault)
		{
			Name = name;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}
	}
}