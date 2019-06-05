# Devil Daggers Survival Editor 1.1.5.0 ([Download](https://devildaggers.info/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor1.1.5.0.zip))

Devil Daggers Survival Editor is a tool that lets you create, view, and edit 'survival' files (also known as spawnsets) for the game Devil Daggers. It is a .NET WPF application built using Visual Studio 2019.

## Main features

- Spawnset editor
- Arena editor with the ability to change tile heights
- Arena presets
- Shrink preview that shows a rough approximation of what the arena will be like at what time during the spawnset
- Easily replacing the currently active spawnset in the game
- Downloading and importing spawnsets directly from https://devildaggers.info

## System requirements

- Microsoft Windows
- .NET Framework 4.6.1 (.NET Framework 4.7.2 for version 2)

## Installation

1. Download the zip file.
2. Unzip its contents.
3. Run DevilDaggersSurvivalEditor.exe inside the folder.

**Note:** You will almost certainly get an automatic warning from Windows saying that you should not run this application because "it has only been downloaded very few times and has an unknown publisher", or something.
The source code is here, so take a look at it if you do not trust it. You could also run the program directly from the source if you wish.

## Links

- [DevilDaggers.info website](https://devildaggers.info)
- [Discord server](https://discord.gg/NF32j8S)

## Changelog

**2.0.0.0 - Work In Progress & To Be Released**

- Rebuilt the entire application from the ground up. The application now makes use of UserControls and proper separation of layout code, WPF binding, reflection, and a lot more stuff that makes the code much more maintainable.
- A lot of performance optimizations.
- The application is now dependent on [DevilDaggersCore](https://bitbucket.org/NoahStolk/devildaggerscore/src/master/), which is a .NET Standard class library used to share code between various Devil Daggers related applications.
- Made the application window resizable.
- Fixed issue where the arena was always incorrectly rotated and mirrored.
- Made user input more lenient, less errors will be thrown and the application will just deal with input values even if they don't make much sense.
- Added new arena presets: Cage Ellipse, Ellipse, Gaps, Islands, and Qbert.
- Added more parameters to existing presets.
- Removed Default Flat preset (can now be created using the new Ellipse preset).
- Renamed and fixed some older presets.
- Added ability to rotate and flip the arena.
- The application warns you when the spawnset you're creating might cause Devil Daggers to become unstable, for instance when the end loop is very short.

**Note:** Version 1 is discontinued. If you wish to view the source code for the latest update for version 1 (which is 1.1.5.0, 1.1.5.1 never got a proper release), the latest commit is from November 4, 2018.

**1.1.5.0 - November 4, 2018**

- Added functionality to automatically check for new versions of the program.

**1.1.4.0 - August 5, 2018**

- Downloading spawnsets and retrieving the spawnset list is now done asynchronously so it doesn't block the application.
- Added functionality to reload the spawnset list if there was no internet connection or if the site was unresponsive.
- Various fixes and small improvements.

**1.1.2.0 - July 27, 2018**

- Added functionality to download spawnsets directly from https://devildaggers.info/Spawnsets within the menu.

**1.0.2.0 - July 26, 2018**

- Enforced en-US globalization.

**1.0.1.0 - June 25, 2018**

- Fixed not being able to read some spawnsets made using a hex editor when reading an undefined enemy type.

**1.0.0.0 - June 16, 2018**

- Initial release.

## How to use

### The basics

In order to understand how to create your own spawnset for Devil Daggers, there are a few basic things that you need to know.

#### The spawns list
- 10 enemies can be spawned in the game. These are Squid I, Squid II, Centipede, Spider I, Leviathan, Gigapede, Squid III, Thorn, Spider II, and Ghostpede (respectively). The order is defined by the survival file structure. All enemies have a one-digit ID.
- Additionally, there is an EMPTY spawn with ID -1. This has no purpose except that the end loop starts after the last EMPTY spawn.
- Every spawn contains an enemy ID and a delay value. The enemy ID (-1 to 9) represents which enemy will be spawned. The delay value represents the amount of seconds between the current spawn and the previous spawn.
- The delay value supports decimal values, even though the original spawnset in the game doesn't use this. This means you can spawn an enemy, for example at 4.5 seconds.
- The end loop is the same set of spawns over and over again, faster every time. How fast it speeds up exactly is explained below, under "Advanced information".
- Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed.

#### Enemies and hand upgrades
- Visit the [devildaggers.info](https://devildaggers.info) wiki pages.

### GUI explanation

#### The arena
- Left click on a tile to enable or disable it.
- Scroll on a tile to change its height.
- Right click on a tile to manually set its height.
- The shrink preview shows a rough approximation of what the arena will be like at which second. This isn't entirely accurate because the application only uses 8x8 pixels per tile.

#### The spawns list
- The end loop is made bold.

### Advanced information

#### The end loop
- This C# console application shows all the end loop spawn seconds for the first 20 waves:
```csharp
using System;

namespace DevilDaggersEndLoop
{
	public static class Program
	{
		public static void Main()
		{
			const float physicsTick = 1 / 60f;

			int[] loopSeconds = new int[] { 5, 8, 11, 16, 21, 22, 23, 33, 34, 35, 40, 41, 46, 51, 56 };

			double waveModifier = 0.0;
			double seconds = 451.0; // loop start

			for (int i = 0; i < 20; i++) // wave index
			{
				double enemyTimer = 0.0;
				for (int j = 0; j < loopSeconds.Length; j++)
				{
					while (enemyTimer < loopSeconds[j])
					{
						seconds += physicsTick;
						enemyTimer += physicsTick + waveModifier;
					}
					Console.WriteLine((Math.Floor(seconds * 60) / 60).ToString("0.0000"));
				}
				waveModifier += physicsTick / 8.0; // After every end wave, each enemy spawns an added 12.5% (100% / 8) faster.
			}
		}
	}
}
```
Thanks to Bintr for figuring out how the end loop speeds up.

#### The arena
- The maximum arena size is 51 by 51.
- The player always spawns at coordinate {25, 25}.
- You can only have 1 tile per coordinate.
- The default arena size is 23 by 23 tiles at the start. This is equivalent to shrink radius 50 (technically the arena would be 25 by 25 but, because of shrinking controls, the outer tiles are already shrunk at the very beginning).
- The arena shrinks in size as time goes on. The default shrink start radius is 50 and the default shrink end radius is 20. The default shrink rate is 0.025. This means the default arena reaches the end radius at 1200 seconds, since (50-20)/0.025 = 1200. (Although not exactly because the shrinking radius will not hit the next tiles exactly at 1200, I haven't bothered with the math behind this, but the last tiles shrink around 1187 seconds. The shrinking technically continues for about 13 seconds but no tiles are affected by it.)
- The original spawnset doesn't use different tile heights, all the tiles are around height 0. (Though there are some tiny differences that are barely noticable, but for convenience we could say that all the tiles are at height 0 and that 0 is the default height.)
- The player can stand on tiles with height -1, but anything lower than that will result in the tile falling down immediately. All tiles below -1 are essentially the same for this reason, so the editor considers any tile with a height below -1 to be "void".
- The tile at coordinate {1, 0} is always invisible for some reason, but you can still walk on it.
- The player can be spawned on different tile heights.
- Enemies and gems do not react to different tile heights, only the player, the daggers, and some effects do (like Thorn spawn smokes, meat chunks, etc).
- The maximum tile height within the editor is set to 63, because anything higher than that seems unnecessary and the main light in the game doesn't reach that far so everything is completely black.
- Setting the tile at coordinate {25, 27} to a value higher than around 0.4975 seems to cause some really odd stuff like audio glitches, invisible hand, and possibly even game crashes.
- The game crashes when you get too near the edge of a full arena (outside of the regular 51x51 range, and I think at or around coordinate {0, 0} but I am not sure).
- The tiles have infinitely long hitboxes, but the texture only covers the top of it.
- 1 tile height is equivalent to 1/4 of a tile (let's say it is a cube). So if you could stack tiles on top of each other, the first tile would be at height 0, the second at height 4, the third at height 8, and so on.
- The player's jump height is equivalent to 1 tile height (1/4 of a tile).
- The player's dagger jump height is just below 5 tile heights. So, from tile height 0, you can dagger jump on a tile with height 4, but not 5.
- The player's double dagger jump height is just below 8 tile heights. So, from tile height 0, you can double dagger jump on a tile with height 7, but not 8.

## Credits

While I've built this application myself, I'd like to thank everyone that has put effort into figuring out how the survival file actually works.
[This thread](https://steamcommunity.com/sharedfiles/filedetails/?id=797571917) posted by ThePassiveDada has helped me a lot.
And also [these two editors](https://steamcommunity.com/app/422970/discussions/0/1483232961033779525/) created by bowsr and Sojk.

## License

MIT License

Copyright (c) 2018-2019 Noah Stolk

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.