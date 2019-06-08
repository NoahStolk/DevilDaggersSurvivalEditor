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
				if (Program.App != null && Program.App.MainWindow != null && Program.App.MainWindow.SpawnsetArena != null)
					Program.App.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsEnabled = !activePreset.IsFull;
			}
		}
		public AbstractArena DefaultPreset { get; private set; }

		public List<AbstractArena> ArenaPresets { get; private set; } = new List<AbstractArena>();

		public IEnumerable<Type> PresetTypes { get; private set; }

		private static readonly Lazy<ArenaPresetHandler> lazy = new Lazy<ArenaPresetHandler>(() => new ArenaPresetHandler());
		public static ArenaPresetHandler Instance => lazy.Value;

		private ArenaPresetHandler()
		{
			PresetTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.Contains("Arena.Presets") && !t.IsAbstract).OrderBy(t => t.Name);

			foreach (Type type in PresetTypes)
				ArenaPresets.Add(Activator.CreateInstance(type) as AbstractArena);

			DefaultPreset = ArenaPresets.Where(a => a.GetType().Name == "Default").FirstOrDefault();
			ActivePreset = DefaultPreset;
		}
	}
}