using DevilDaggersSurvivalEditor.Arena.Presets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevilDaggersSurvivalEditor.Arena
{
	/// <summary>
	/// Singleton class to create all arena presets automatically through reflection and keep them in memory rather than re-instantiating them every time so their settings are remembered.
	/// </summary>
	public sealed class ArenaPresetHandler
	{
		private AbstractArena _activePreset;
		private static readonly Lazy<ArenaPresetHandler> _lazy = new Lazy<ArenaPresetHandler>(() => new ArenaPresetHandler());

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		private ArenaPresetHandler()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			PresetTypes = App.Assembly
				.GetTypes()
				.Where(t =>
					t.FullName?.Contains("Arena.Presets", StringComparison.InvariantCulture) == true &&
					t.FullName?.Contains("DisplayClass", StringComparison.InvariantCulture) == false &&
					!t.IsAbstract)
				.OrderBy(t => t.Name);

			foreach (Type type in PresetTypes)
			{
				if (Activator.CreateInstance(type) is AbstractArena arena)
					ArenaPresets.Add(arena);
			}

			DefaultPreset = ArenaPresets.FirstOrDefault(a => a.GetType().Name == "Default") ?? throw new("Could not find default arena preset.");
			ActivePreset = DefaultPreset;
		}

		public static ArenaPresetHandler Instance => _lazy.Value;

		public AbstractArena ActivePreset
		{
			get => _activePreset;
			set
			{
				_activePreset = value;
				if (App.Instance?.MainWindow?.SpawnsetArena != null)
					App.Instance.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsEnabled = !_activePreset.IsFull;
			}
		}

		public AbstractArena DefaultPreset { get; }

		public List<AbstractArena> ArenaPresets { get; } = new List<AbstractArena>();

		public IEnumerable<Type> PresetTypes { get; }
	}
}