﻿using DevilDaggersSurvivalEditor.Code.ArenaPresets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevilDaggersSurvivalEditor.Code
{
	/// <summary>
	/// Singleton class to create all arena presets automatically through reflection and keep them in memory rather than re-instantiating them every time so their settings are remembered.
	/// </summary>
	public sealed class ArenaPresetHandler
	{
		public AbstractArena Preset { get; set; }

		public readonly List<AbstractArena> ArenaPresets = new List<AbstractArena>();

		private static readonly Lazy<ArenaPresetHandler> lazy = new Lazy<ArenaPresetHandler>(() => new ArenaPresetHandler());
		public static ArenaPresetHandler Instance => lazy.Value;

		private ArenaPresetHandler()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.Contains("ArenaPresets") && !t.IsAbstract))
				ArenaPresets.Add(Activator.CreateInstance(type) as AbstractArena);
		}
	}
}