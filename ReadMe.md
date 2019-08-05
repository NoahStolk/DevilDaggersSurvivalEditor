# Devil Daggers Survival Editor 2.4.0.0 ([Download](https://devildaggers.info/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor2.4.0.0.zip))

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
- DevilDaggersSurvivalEditor uses pixel shaders. In order to build pixel shaders you will need Microsoft's DirectX SDK from June 2010, and use the FXC compiler tool. If you do not need to build pixel shaders (the build for the origin one is already included in the source), then you can skip this step and remove the pre-build event from Build Events in DevilDaggersSurvivalEditor.csproj's properties to prevent it from throwing an error.

## Links

- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersSurvivalEditor](https://devildaggers.info/Tools/DevilDaggersSurvivalEditor)
- [List of spawnsets, as well as general information about how spawnsets work](https://devildaggers.info/Spawnsets)
- [Discord server](https://discord.gg/NF32j8S)

## Changelog

#### 2.4.0.0 - August 5, 2019

- Added end loop preview which allows you to view spawn information for any wave. This includes Gigapedes changing into Ghostpedes every third wave. The end loop preview can also be turned off in the Settings window.
- Added ability to increment/decrement tile heights by 0.1 while holding CTRL and using the mouse wheel.
- Added functionality to auto-detect the location of the survival file within the Settings window.
- Recreated layout for Settings window.
- Optimised performance when deleting spawns.
- Added "clear search" buttons to Download Spawnset window.
- Added "leaderboard" column to the spawnset list in the Download Spawnset window.
- Improved Download Spawnset window layout.
- Added tooltips that displays a spawnset's description if it has one.
- Fixed log file not being written to.
- Other layout improvements, small bug fixes, and improvements.

#### 2.3.1.0 - July 30, 2019

- Fixed crash when deleting multiple spawns.

#### 2.3.0.0 - July 30, 2019

- Heavily optimised performance for the spawns section.
- Added author sorting to the Download Spawnset window.
- Fixed the application not asking whether you want to save the current spawnset when closing the application.
- Fixed Modify Spawn Delay window being able to change spawn delay values into negative values.
- The application now displays the Update Recommended window on start up when an update is available.
- Layout improvements for the Download Spawnset window.

#### 2.2.0.0 - July 7, 2019

- The application now keeps track of whether or not you have any unsaved changes and will ask you whether or not you want to to save it before proceeding to overwrite it by opening an existing spawnset or creating a new one.
- The application window title now displays the current spawnset's name if it has one.
- Added Save As menu item.
- Added shortcut keys:
	- CTRL+S - Save
	- CTRL+C - Copy currently selected spawn(s)
	- CTRL+V - Paste spawn(s) currently on the clipboard
	- Delete - Delete currently selected spawn(s)
- The Download Spawnset window now remembers the spawnset sorting after it is closed.
- The application now asks you to confirm to overwrite the existing arena with a preset, as this cannot be undone easily.
- The maximum amount of spawns you can have per spawnset is now set to 10,000.
- Improved messages when saving or replacing spawnsets.
- Fixed the survival file restore writing the original file bytes on top of the file instead of overwriting it entirely.
- Fixed the application not displaying an "unsaved changes" warning message when opening the currently active survival file.
- Fixed the end loop not being displayed correctly when there are no EMPTY spawns in the spawnset.
- Performance optimisations, layout improvements, and other bug fixes.

#### 2.1.1.0 - July 5, 2019

- Hotfix for restoring the default survival file.

#### 2.1.0.0 - July 5, 2019

- Added spawnset sorting to the Download Spawnset window.
- Added Select All and Deselect All buttons to the arena editor.
- Added copy/paste functionality to the spawns editor.
- Added timeout for web requests so the application doesn't keep waiting when the website is offline.
- Fixed tile elements not updating colours after rounding or randomising.
- The spawns editor scrollbar now scrolls to the end when adding new spawns.
- The Download Spawnset window now remembers the author and spawnset search values after it is closed.
- The default survival file is now embedded into the executable so the actual file is not needed anymore. This removes the issue where the application crashes whenever the file would not be present.
- Optimisations and layout improvements.

#### 2.0.0.0 - June 29, 2019

Devil Daggers Survival Editor 2 is a complete rewrite and redesign of the entire application.

- General
	- Made the main application window resizable (includes fullscreen).
	- Added loading screen.
	- Made user input more lenient, less errors will be thrown and the application will just deal with input values even if they don't make much sense.
- Fixes
	- Fixed issue where the arena was always incorrectly rotated and mirrored.
	- Fixed issue where restoring the original V3 spawnset would remove the survival file from the application's 'Content' folder and as a result the option no longer works until the file is put back (either by re-downloading the application or by doing it manually).
	- Fixed issue where the application would not start up again after using it (due to the issue above).
	- Fixed issue where you could create spawns with a negative delay value.
- Spawns
	- Added ability to modify (add, subtract, multiply, divide) delays for selected spawns. This can be used to easily speed up or slow down parts of a spawnset, or a spawnset in its entirety.
	- Added ability to switch enemy types for selected spawns.
	- Added ability to add or insert the same spawn multiple times at once using the Amount value.
- Arena
	- Set the max tile height to 54 rather than 63, since any tiles with a height value greater than 54 will be in complete darkness and won't be visible (regardless of the spawnset brightness setting, or the in-game gamma setting).
	- The arena editor now shows void tile heights as "Void" rather than their actual (meaningless) value.
	- Added height selector which lets you pick a height and use it in the arena editor.
	- Removed the old height map as this is now redundant.
	- Added multiple tile selection to the arena editor.
	- Added continuous tile modification and selection to the arena editor.
	- Added rectangular tile modification and selection to the arena editor.
	- Added ability to round heights for selected tiles.
	- Added ability to randomise heights for selected tiles.
	- Added ability to rotate and flip the arena.
	- Made the tiles brighter for better visibility.
	- Optimised the shrink preview slider for better performance.
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
	- Removed Default Flat preset, as it can now be created using the new Ellipse preset, or using the Round Heights button on the Default arena.
	- Added wall thickness parameter to Cage Rectangular preset.
	- Added offset parameters to Qbert preset.
	- Added option for arena presets whether to overwrite the previous arena entirely or to generate new tiles on top of it.
- Menu
	- Replaced the Open From DevilDaggers.info menu item with a new Download Spawnset window which contains the spawnsets list.
	- Added menu item to open the current survival file.
- Spawnsets from DevilDaggers.info
	- Added search and filter options to the spawnsets list.
	- The spawnsets list now shows more information (such as when the loop starts) about the spawnsets.
- Miscellaneous
	- Added various warnings:
		- The application warns you when the spawnset you're creating might cause Devil Daggers to become unstable, for instance when the end loop is very short, or when you're spawning the player in the void. This also includes the new discovery of the {25, 27} tile, which causes Devil Daggers to glitch whenever its height is put to a value greater than 0.4973333.
		- The application warns you when the path to the survival file in the user settings is incorrect, or when the file could not be parsed.
	- Added more settings:
		- Prevent the player from spawning in the void by making sure the spawn tile always has a height.
		- Prevent tile {25, 27} from going outside of its safe range.
	- The application now uses logging, so whenever it crashes you can open the log to see what went wrong.
	- The application is now dependent on [DevilDaggersCore](https://bitbucket.org/NoahStolk/devildaggerscore/src/master/), which is a .NET Standard class library used to share code between various Devil Daggers related applications.

#### 1.1.5.0 - November 4, 2018

- Added functionality to automatically check for new versions of the program.

#### 1.1.4.0 - August 5, 2018

- Downloading spawnsets and retrieving the spawnset list is now done asynchronously so it doesn't block the application.
- Added functionality to reload the spawnset list if there was no internet connection or if the site was unresponsive.
- Various fixes and small improvements.

#### 1.1.2.0 - July 27, 2018

- Added functionality to download spawnsets directly from [DevilDaggers.info/Spawnsets](https://devildaggers.info/Spawnsets) within the menu.

#### 1.0.2.0 - July 26, 2018

- Enforced en-US globalisation.

#### 1.0.1.0 - June 25, 2018

- Fixed not being able to read some spawnsets made using a hex editor when reading an undefined enemy type.

#### 1.0.0.0 - June 16, 2018

- Initial release.

#### 0.9.0.0 - May 14, 2018

- Beta release.

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