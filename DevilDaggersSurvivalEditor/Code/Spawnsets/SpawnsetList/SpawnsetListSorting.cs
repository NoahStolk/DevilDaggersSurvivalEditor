using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class SpawnsetListSorting<T> where T : AbstractListEntry
	{
		public string FullName { get; }
		public string DisplayName { get; }
		public Func<T, object> SortingFunction { get; }
		public bool IsAscendingDefault { get; }

		public bool Ascending { get; set; }

		public SpawnsetListSorting(string fullName, string displayName, Func<T, object> sortingFunction, bool isAscendingDefault)
		{
			FullName = fullName;
			DisplayName = displayName;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}
	}
}