# Devil Daggers Survival Editor 0.9 (beta)

Devil Daggers Survival Editor is a tool that lets you create, view, edit, and switch 'survival' files (also known as spawnsets) for the game Devil Daggers.

- Spawnset editor
- Arena editor with the ability to change tile heights
- Arena presets (WIP)
- Shrink preview that shows a rough approximation of what the arena will be like at what time during the spawnset
- Easily replacing the currently active spawnset in the game

## System requirements

- Microsoft Windows
- .NET 3.0 or above

## Installation

1. Download DevilDaggersSurvivalEditor.zip.
2. Unzip the contents of DevilDaggersSurvivalEditor.zip.
3. Run DevilDaggersSurvivalEditor.exe inside the folder.

**Note:** You will almost certainly get an automatic warning from Windows saying that you should not run this application because "it has only been downloaded very few times and has an unknown publisher", or something.
Just take a look at the source code if you do not trust the application.

## How to use

### The basics

In order to understand how to create your own spawnset for Devil Daggers, there are a few basic things that you need to know.

#### The spawns list
- 10 enemies can be spawned in the game: Squid I, Squid II, Centipede, Spider I, Leviathan, Gigapede, Squid III, Thorn, Spider II, Ghostpede. (Order is defined by the survival file structure, all enemies have a one-digit ID.)
- Additionally there's an EMPTY spawn with ID -1. This has no purpose except that the end loop starts after the last EMPTY spawn.
- The end loop is basically the same set of spawns over and over again, faster every time.
- Every third wave all Gigapedes are changed into Ghostpedes.
- Every spawn contains one enemy and a delay value, the delay value represents the amount of seconds between the current spawn and the previous spawn.
- The delay value supports decimal values, even though the game doesn't use this. This means you can spawn an enemy, for example every 4.5 seconds.

#### Enemies and hand upgrades
- Visit [devildaggers.info](http://devildaggers.info).

### Advanced information

#### The end loop
- This C# console application shows all the end loop seconds for the first 20 waves: (Someone please translate this to human language.)
```
using System;
using System.Collections.Generic;

namespace DevilDaggersEndLoop
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			List<int> loopSeconds = new List<int> { 5, 8, 11, 16, 21, 22, 23, 33, 34, 35, 40, 41, 46, 51, 56 };

			double waveModifier = 0.0;
			double seconds = 451.0; // loop start

			for (int i = 0; i < 20; i++) // wave index
			{
				double enemyTimer = 0.0;
				for (int j = 0; j < loopSeconds.Count; j++)
				{
					while (enemyTimer < loopSeconds[j])
					{
						seconds += 1.0 / 60.0;
						enemyTimer += 1.0 / 60.0 + waveModifier;
					}
					Console.WriteLine(seconds.ToString("0.0000"));
				}
				waveModifier += 0.00208333; // hardcoded value in the game
			}
		}
	}
}
```
Thanks to Bintr for figuring this out.

#### The arena
- The default arena size is 23 by 23 tiles at the start. This is equivalent to shrink radius 50 (technically the arena would be 25 by 25 but because of shrinking controls the outer tiles are already shrunk at the very beginning).
- The arena shrinks in size as time goes on. The default shrink start radius is 50 and the default shrink end radius is 20. The default shrink rate is 0.025. So the default arena reaches the end radius at 1200 seconds because (50-20)/0.025 = 1200. (Although not exactly because the shrinking radius will not hit the next tiles exactly at 1200, I haven't bothered with the math behind this, but the last tiles shrink around 1187 seconds. The shrinking technically continues for about 13 seconds but no tiles are affected by it.)
- The maximum arena size is 50 by 50.
- The player always spawns at coordinate 25,25.
- The original game doesn't use different tile heights, all the tiles are around height 0 (there are some tiny differences that are barely noticable, but for convenience we could say that all the tiles are at height 0 and that 0 is the default height).
- The player can stand on tiles with height -1, but anything lower than that will result in dying (FALLEN). The default "void" height in the original game is around -1007.57, but for convenience I use the number -1000 because it doesn't matter. All tiles below -1 are essentially the same because you cannot see them or stand on them without dying.
- The tile at coordinate 0,1 is invisible for some reason, but you can still walk on it.
- The player can be spawned on different tile heights, though this is not recommended because some enemies will go through the floor, you won't be able to pick up any gems (they only fly towards height 0), and some really odd stuff can happen (audio glitches, no hand appearing, crashes...).
- I set the maximum tile height of the editor to 63 because odd stuff happens when the player reaches a certain height...
- The game crashes when you get too near the edge of a full arena (below x/y 0 or above x/y 50, and I think at/around coordinate 0,0 but I am not sure).
- The tiles have infinitely long hitboxes but the texture only covers the top of it.
- You can only have 1 tile per coordinate.
- 1 tile height is equivalent to 1/4 of a tile (let's say it is a cube). So if you could stack tiles on top of each other, the first tile would be at height 0, the second at height 4, the third at height 8, and so on.
- The player's jump height is equivalent to 1 tile height (1/4 of a tile).
- The player's dagger jump height is just below 5 tile heights. So, from tile height 0, you can dagger jump on a tile with height 4, but not 5.
- The player's double dagger jump height is just below 8 tile heights. So, from tile height 0, you can double dagger jump on a tile with height 7, but not 8.

### GUI explanation

#### The arena
- Left click on a tile to disable/enable.
- Scroll on a tile to change its height.
- The shrink preview shows a rough approximation of what the arena will be like at which second. This isn't entirely accurate because the application only uses 8x8 pixels per tile.

#### The spawns list
- The end loop is made bold.

## Credits

While I've made this application myself, I'd like to thank everyone that has put effort into figuring out how the survival file actually works.
[This thread](https://steamcommunity.com/sharedfiles/filedetails/?id=797571917) posted by ThePassiveDada has helped me a lot.
And also [these two editors](https://steamcommunity.com/app/422970/discussions/0/1483232961033779525/) created by bowsr and Sojk.

## License

MIT License

Copyright (c) 2018 Noah Stolk

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