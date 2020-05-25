using DevilDaggersSurvivalEditor.Code.Arena.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevilDaggersSurvivalEditor.Code.Arena
{
	/// <summary>
	/// Singleton class to create all arena presets automatically through reflection and keep them in memory rather than re-instantiating them every time so their settings are remembered.
	/// </summary>
	public sealed class ArenaPresetHandler
	{
		private AbstractArena activePreset;
		public AbstractArena ActivePreset
		{
			get => activePreset;
			set
			{
				activePreset = value;
				if (App.Instance != null && App.Instance.MainWindow != null && App.Instance.MainWindow.SpawnsetArena != null)
					App.Instance.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsEnabled = !activePreset.IsFull;
			}
		}
		public AbstractArena DefaultPreset { get; private set; }

		public List<AbstractArena> ArenaPresets { get; private set; } = new List<AbstractArena>();

		public IEnumerable<Type> PresetTypes { get; private set; }

		private static readonly Lazy<ArenaPresetHandler> lazy = new Lazy<ArenaPresetHandler>(() => new ArenaPresetHandler());
		public static ArenaPresetHandler Instance => lazy.Value;

		private ArenaPresetHandler()
		{
			PresetTypes = App.Assembly.GetTypes().Where(t => t.FullName.Contains("Arena.Presets") && !t.IsAbstract && !t.Attributes.HasFlag(TypeAttributes.NestedPrivate)).OrderBy(t => t.Name);

			foreach (Type type in PresetTypes)
				if (Activator.CreateInstance(type) is AbstractArena arena)
					ArenaPresets.Add(arena);

			DefaultPreset = ArenaPresets.FirstOrDefault(a => a.GetType().Name == "Default");
			ActivePreset = DefaultPreset;
		}
	}
}