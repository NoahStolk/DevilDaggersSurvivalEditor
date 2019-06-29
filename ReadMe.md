# Devil Daggers Survival Editor 1.1.5.0 ([Download](https://devildaggers.info/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor1.1.5.0.zip))

Devil Daggers Survival Editor is a tool that lets you create, view, and edit 'survival' files (also known as spawnsets) for the game Devil Daggers. It is a .NET WPF application built using Visual Studio 2019.

## Main features

- Spawnset editor
- Arena editor with the ability to change tile heights
- Arena presets
- Shrink preview that shows a rough approximation of what the arena will be like at what time during the spawnset
- Easily replacing the currently active spawnset in the game
- Downloading and importing spawnsets directly from [DevilDaggers.info](https://devildaggers.info)

## System requirements

- Microsoft Windows
- .NET Framework 4.6.1

## Installation

1. Download the zip file.
2. Unzip its contents.
3. Run DevilDaggersSurvivalEditor.exe inside the folder.

## Running from source

- DevilDaggersSurvivalEditor is dependent on [NetBase](https://bitbucket.org/NoahStolk/netbase/src/master/) and [DevilDaggersCore](https://bitbucket.org/NoahStolk/devildaggerscore/src/master/). Their sources need to be within the same root folder as the source for DevilDaggersSurvivalEditor itself.
- DevilDaggersSurvivalEditor uses pixel shaders. In order to build pixel shaders you will need Microsoft's DirectX SDK from June 2010, and use the FXC compiler tool. If you do not need to build pixel shaders (the build for the origin one is already included in the source), then you can remove the pre-build event from Build Events in DevilDaggersSurvivalEditor.csproj's properties to prevent it from throwing an error.

## Links

- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersSurvivalEditor](https://devildaggers.info/Tools/DevilDaggersSurvivalEditor)
- [Online list of spawnsets, as well as general information about how spawnsets work](https://devildaggers.info/Spawnsets)
- [Discord server](https://discord.gg/NF32j8S)

## Changelog

#### 2.0.0.0 - Work In Progress & To Be Released

Devil Daggers Survival Editor 2 is a complete rewrite and redesign of the entire application.

- General
	- Made the main application window resizable (includes fullscreen).
	- Added loading screen.
	- Made user input more lenient, less errors will be thrown and the application will just deal with input values even if they don't make much sense.
- Fixes
	- Fixed issue where the arena was always incorrectly rotated and mirrored.
	- Fixed issue where restoring the original V3 spawnset would remove the survival file from the application's 'Content' folder and as a result the option no longer works until the file is put back (either by re-downloading the application or by doing it manually).
	- Fixed issue where the application would not start up after using it (not confirmed yet).
	- Fixed issue where you could create spawns with a negative delay value.
- Spawns
	- Added ability to modify (add, subtract, multiply, divide) delays for selected spawns. This can be used to easily speed up or slow down parts of a spawnset, or a spawnset in its entirety.
	- Added ability to switch enemy types for selected spawns.
	- Added ability to add or insert the same spawn multiple times at once using the "Amount" value.
- Arena
	- Set the max tile height to 54 rather than 63, since any tiles with a height value greater than 54 will be in complete darkness and won't be visible (regardless of the spawnset brightness setting, or the in-game gamma setting).
	- The arena editor now shows void tile heights as "Void" rather than their actual (meaningless) value.
	- Added height selector which lets you pick a height and use it in the arena editor.
	- Removed the old height map as this is now redundant.
	- Added multiple tile selection to the arena editor.
	- Added continuous tile modification and selection to the arena editor.
	- Added rectangular tile modification and selection to the arena editor.
	- Added ability to round heights for selected tiles.
	- Added ability to randomize heights for selected tiles.
	- Added ability to rotate and flip the arena.
	- Made the tiles brighter for better visibility.
	- Optimized the shrink preview slider for better performance.
	- Implemented custom pixel shading for the arena editor to take advantage of high-performant GPU rendering to render lighting, selection borders and selection highlighting.
- Arena presets
	- Renamed Pyramid preset to Qbert.
	- Renamed Cage preset to Cage Rectangular.
	- Renamed Random preset to Random Noise.
	- Added new arena presets:
		- Cage Ellipse
		- Ellipse
		- Hill
		- Pyramid
		- Random Gaps
		- Random Islands
		- Random Pillars
	- Removed Default Flat preset, as it can now be created using the new Ellipse preset, or using the "Round heights" button on the Default arena.
	- Added wall thickness parameter to Cage Rectangular preset.
	- Added offset parameters to Qbert preset.
	- Added option for arena presets whether to overwrite the previous arena entirely or to generate new tiles on top of it.
- Menu
	- Replaced the "Open from DevilDaggers.info" menu item with a new "Download Spawnset" window which contains the online spawnsets list.
	- Added menu item to open the current survival file.
- Online spawnsets
	- Added search and filter options to the online spawnsets list.
	- The online spawnsets list now shows more information (such as when the loop starts) about the spawnsets.
- Miscellaneous
	- Added various warnings:
		- The application warns you when the spawnset you're creating might cause Devil Daggers to become unstable, for instance when the end loop is very short, or when you're spawning the player in the void. This also includes the new discovery of the {25, 27} tile, which causes Devil Daggers to glitch whenever its height is put to a value greater than 0.4973333.
		- The application warns you when the path to the survival file in the user settings is incorrect, or when the file could not be parsed.
	- Added more settings:
		- Prevent the player from spawning in the void.
		- Prevent tile {25, 27} from going outside of its safe range.
	- The application now uses logging, so whenever it crashes you can open the log to see what went wrong.
	- The application is now dependent on [DevilDaggersCore](https://bitbucket.org/NoahStolk/devildaggerscore/src/master/), which is a .NET Standard class library used to share code between various Devil Daggers related applications.

**Note:** Version 1 is discontinued. If you wish to view the source code for the latest update for version 1 (which is 1.1.5.0, since 1.1.5.1 never got a proper release), the latest commit is from November 4, 2018.

#### 1.1.5.0 - November 4, 2018

- Added functionality to automatically check for new versions of the program.

#### 1.1.4.0 - August 5, 2018

- Downloading spawnsets and retrieving the spawnset list is now done asynchronously so it doesn't block the application.
- Added functionality to reload the spawnset list if there was no internet connection or if the site was unresponsive.
- Various fixes and small improvements.

#### 1.1.2.0 - July 27, 2018

- Added functionality to download spawnsets directly from [DevilDaggers.info/Spawnsets](https://devildaggers.info/Spawnsets) within the menu.

#### 1.0.2.0 - July 26, 2018

- Enforced en-US globalization.

#### 1.0.1.0 - June 25, 2018

- Fixed not being able to read some spawnsets made using a hex editor when reading an undefined enemy type.

#### 1.0.0.0 - June 16, 2018

- Initial release.

## Credits

While I've built this application myself, I'd like to thank everyone that has put effort into figuring out how the survival file actually works.
[This thread](https://steamcommunity.com/sharedfiles/filedetails/?id=797571917) posted by ThePassiveDada has helped me a lot, as well as [these two editors](https://steamcommunity.com/app/422970/discussions/0/1483232961033779525/) created by bowsr and Sojk.

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