using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class SpawnsetListSorting<T> where T : class
	{
		public string Name { get; }
		public Func<T, object> SortingFunction { get; }
		public bool IsAscendingDefault { get; }

		public bool Ascending { get; set; }

		public SpawnsetListSorting(string name, Func<T, object> sortingFunction, bool isAscendingDefault)
		{
			Name = name;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}
	}
}