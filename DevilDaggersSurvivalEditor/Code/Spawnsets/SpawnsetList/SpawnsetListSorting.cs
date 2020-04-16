using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	internal class SpawnsetListSorting<T> where T : AbstractListEntry
	{
		internal string FullName { get; }
		internal string DisplayName { get; }
		internal Func<T, object> SortingFunction { get; }
		internal bool IsAscendingDefault { get; }

		internal bool Ascending { get; set; }

		internal SpawnsetListSorting(string fullName, string displayName, Func<T, object> sortingFunction, bool isAscendingDefault)
		{
			FullName = fullName;
			DisplayName = displayName;
			SortingFunction = sortingFunction;
			IsAscendingDefault = isAscendingDefault;
		}
	}
}