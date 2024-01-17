# Changelog

## [2.47.0.0] - 2024-01-17

This is an API compatibility update. Older versions will soon stop working.

- The application now uses the latest API endpoints so that it can continue to function.
- The application no longer checks for updates.
- The application no longer displays the changelog.

## [2.46.1.0] - 2022-08-26

- The arena shrink preview is now accurate. This also applies to the 'Set tiles outside of arena shrink start to void` button.
- The arena is now rendered using the same colors as on the website.
- Fixed default arena being flipped incorrectly. This fix applies to the 'Default' arena preset, the default state when opening the app, and creating a new file from the menu.

## [2.45.0.0] - 2022-05-01

- API updates to support multiple builds. Windows 7 now has a separate build. More builds for other operating systems will follow in the future (not soon).
- Fixed settings not saving after exiting the app if the settings window wasn't opened during that session.

## [2.44.0.0] - 2022-03-07

- Added support for the Race game mode.
- Added error message on launch when you have a folder named 'survival' in the mods folder.

## [2.42.2.0] - 2022-01-09

- The application now runs on .NET 6 which is a lot faster and will be officially supported until late 2024.
- You do not need to install the .NET runtime anymore. You do not need to install anything for the program to work as of this update (unless you're running Windows 7).
- Added button to clean up tiles outside of arena shrink range.
- Fixed not updating effective player settings on load.

## [2.41.1.0] - 2021-11-16

- Fixed crash when pressing delete in spawns list when nothing is selected.

## [2.41.0.0] - 2021-10-15

- Application now allows all possible values for arena shrink, brightness, additional gems, and start timer.
- Removed checkbox for disabling gem collection since you can now set the value for additional gems to negative.
- Added effective hand and gems/homing to layout. For example, when you set the hand to Level 1 and set the additional gems to 70, the effective result is Level 3 hand with 0 homing. This also replaces the warning stating that negative additional gems for Level 2 will reset the hand to Level 1.
- Effective hand and gems/homing are now shown in the 'Download spawnset' window instead of the spawnset hand and additional gems values.
- Effective gems/homing is now used in the 'Total gems' values in the spawns list.
- Tile heights are no longer restricted between values -1 and 54 in arena presets and arena editor.
- Added tile height button with value -1.01.
- Fixed incorrect arena tile preview when shrink end is higher than shrink start.
- Fixed layout issue with long spawnset or author names in the 'Download spawnset' window.
- Removed 32-bit support completely because Devil Daggers does not run on 32-bit machines anymore.

## [2.34.0.0] - 2021-04-26

- Added game version and game mode to the 'Download spawnset' window.
- Fixed loading TimeAttack spawnsets from site when end loop preview was enabled.
- Spawns are no longer bold (indicating end loop) for TimeAttack spawnsets.

## [2.33.0.0] - 2021-04-25

- Added support for 'TimeAttack' game mode.
- Added new 'Diamond' arena preset.

## [2.31.9.0] - 2021-04-20

- Minor fixes for the 'Download spawnset' window.

## [2.31.6.0] - 2021-04-14

- Fixed UI lag for additional gems and timer start.
- Fixed occasionally overwriting internal spawnset values caused by programmatic UI updates.
- Added warning for disabling gem collection and setting hand to Level 2.

## [2.31.2.0] - 2021-04-11

- The application now allows you to disable gem collection which adds the possibility for more interesting spawnsets.
- Fixed not always updating display spawn total gems correctly.

## [2.30.5.0] - 2021-03-18

- Fixed empty settings or cache files crashing the application.
- Fixed About window crashing.
- Removed Help window. The Help menu item now navigates to the Survival Editor Guide on the website.

## [2.30.2.0] - 2021-03-15

- Fixed potential uncaught exception caused by empty cached sorting directions.

## [2.30.1.0] - 2021-03-13

- Implemented caching for the 'Download spawnset' window. The application now remembers which sortings and filters were applied after closing the window, and also after closing the application.
- Added shortcuts for switching between spawnset pages in the 'Download spawnset' window. (Arrow keys cannot be used for technical reasons.)
  - Use CTRL , to go to first page
  - Use , to go to previous page
  - Use . to go to next page
  - Use CTRL . to go to last page

## [2.28.18.0] - 2021-03-10

- Added support for reading and writing older survival formats. You can open and save V1 spawnsets and select the format you want to use in the editor.
- Added cancellation to confirmation windows. Clicking [X] on a confirmation window now cancels the operation properly.
- Rewrote 'Download spawnset' window. Now includes 'Custom leaderboard' and 'Practice' filtering, paging, improved and extended sorting, and improved performance.
- User settings are now saved as settings.json in the local application data Windows folder (access by pressing Windows R and typing %localappdata%).
- Added keyboard shortcuts for every menu item under 'File'.
- Added menu item to open default V3 survival file.
- New V3.1 spawnset settings now have their own section.
- Added [X] button to Settings window.
- Removed glitch tile (25,27) setting, workaround, and warning. This seems to be fixed in V3.1.
- Improved performance for updating spawns list.
- Improved performance for individual spawn controls.
- Warnings are now more apparent.
- Fixed keyboard shortcut bugs.
- Removed max spawns limit. Crash at your own risk.
- Fixed bug where end loop timings would sometimes not update properly when editing start timer.
- Improved keyboard shortcut support.
- Improved logo.

## [2.15.0.0] - 2021-02-24

- Implemented TimerStart value which can be helpful for practice. This timer is only used as the initial start value for the in-game timer.

## [2.14.7.0] - 2021-02-22

- Added support for new practice values. You can now change the initial hand upgrade, as well as the amount of additional gems, in the survival file itself. This makes spawnsets significantly more interesting.
- Custom spawnsets are now saved to the "mods" directory to keep the original survival file intact. You do not need to modify the original survival file anymore. When you are done playing a custom spawnset, just delete the file to go back to the original game.
- The application now runs on .NET 5.0.
- Added more keyboard shortcuts for menu items.
- Functionality to quickly restore the original V3 spawnset has been replaced by a new option that lets you delete the survival mod file.
- Removed validation of the survival file on launch.
- Replaced "no survival file" warning with "invalid Devil Daggers root folder" warning.
- Added AskToReplaceSurvivalFile and AskToDeleteSurvivalFile user settings.

## [2.10.13.0] - 2020-10-07

- Fixed "Download" button in UpdateRecommended window navigating to an outdated URL.
- Removed misleading information from the Help window.

## [2.10.11.0] - 2020-09-24

- The application now uses a custom dark theme. General layout for many components has been improved as well.
- Added Donut arena preset.
- Added new user settings.
  - Ask to confirm arena generation. When generating an arena with a preset the application prompts if you really want to replace the current arena. This can now be turned off with this new setting so you don't have to click "Yes" every time, which is convenient when playing with presets that involve randomness.
  - Replace survival file with downloaded spawnset. When downloading a spawnset from within the application, it will ask if you want to replace the current survival file with the downloaded spawnset. This can now be done automatically, or not at all, using this new setting. You can set it to "Always", "Ask", or "Never".
  - Load current survival file on start up.
- Removed functionality to open log file from the menu as this doesn't work well with the current logging mechanism.
- Spawn seconds are now properly rounded to physics ticks in Devil Daggers (1/60 of a second).
- Selecting an enemy type is now done with radio buttons rather than a dropdown.
- Selecting a delay modification function in the ModifySpawnDelay window is now done with radio buttons rather than a dropdown.
- Fixed crash when selecting all tiles.
- Fixed 'No survival file' warning not updating after writing a spawnset to the file.
- Fixed CheckingForUpdates window not waiting for the API result properly.
- Fixed being able to divide delay by zero in the ModifySpawnDelay window.

## [2.7.6.1] - 2020-08-20

- Rebuilt application for API updates.

## [2.7.6.0] - 2020-08-20

- Rewrote much of the application.
- Removed dependencies.
- Ported to .NET Core. The application is no longer dependent on .NET Framework and does not require .NET Framework 4.6.1.
- Fixed 'No survival file' warning not updating after restoring V3.
- Fixed clicking settings CheckBoxes text not affecting the CheckBox states.
- Fixed end loop warning seconds not being formatted correctly.
- Fixed not showing end loop warning for a single spawn.
- Fixed crash when attempting to view log file while it was being used by another process.
- Fixed search TextBoxes in the 'Download spawnset' window being empty after reopening the window.
- Improved startup performance by reducing the amount of API calls.

## [2.4.16.0] - 2020-06-10

- Added 'Set' function to Modify Spawn Delay window.
- Fixed clicking 'Clear previous tiles' text not affecting the CheckBox state.
- Maintenance and small performance improvements.

## [2.4.13.1] - 2019-11-18

- Updated source code URL.

## [2.4.13.0] - 2019-11-03

- Changelog now indicates the currently running version.
- Small changes related to maintenance of the website, as well as code refactoring and improvements.
- Improved layout for Error window.

## [2.4.10.0] - 2019-10-22

- Added Changelog window.
- Fixed the Update Recommended window not closing after clicking the download button.
- Small changes related to maintenance of the website, as well as code refactoring and improvements.
- Memory scanning code has been moved to Devil Daggers Core because the idea of merging Devil Daggers Survival Editor and Devil Daggers Custom Leaderboards was cancelled.

## [2.4.4.0] - 2019-09-01

- Removed "Loop start" column from the online spawnset list.
- The Switch Enemy Type window now only displays enemy types that exist in the current spawn selection.
- Removed minimum window size and added scrollbars that become active when the window size becomes smaller than 1366x768. This applies to the Main window and the Download Spawnset window.
- Fixed spawn enemy text color not always updating correctly.
- Small layout improvements.
- Improved logging.
- Internal changes such as importing Devil Daggers Custom Leaderboards specific code for memory scanning base functionality as preparation for possible custom leaderboard integration, as well as general code refactoring and improvements.

## [2.4.0.0] - 2019-08-05

- Added end loop preview which allows you to view spawn information for any wave. This includes Gigapedes changing into Ghostpedes every third wave. The end loop preview can also be turned off in the Settings window.
- Added ability to increment/decrement tile heights by 0.1 while holding CTRL and using the mouse wheel.
- Added functionality to auto-detect the location of the survival file within the Settings window.
- Recreated layout for Settings window.
- Optimized performance when deleting spawns.
- Added "clear search" buttons to Download Spawnset window.
- Added "leaderboard" column to the spawnset list in the Download Spawnset window.
- Improved Download Spawnset window layout.
- Added tooltips that display a spawnset's description if it has one.
- Fixed log file not being written to.
- Other small layout changes, bug fixes, and improvements.

## [2.3.1.0] - 2019-07-30

- Fixed crash that occurred when deleting multiple spawns.

## [2.3.0.0] - 2019-07-30

- Heavily optimized performance for the spawns section.
- Added author sorting to the Download Spawnset window.
- Fixed the application not asking whether you want to save the current spawnset when closing the application.
- Fixed Modify Spawn Delay window being able to change spawn delay values into negative values.
- The application now displays the Update Recommended window on start up when an update is available.
- Layout improvements for the Download Spawnset window.

## [2.2.0.0] - 2019-07-07

- The application now keeps track of whether or not you have any unsaved changes and will ask you if you want to save the spawnset before proceeding to overwrite it by opening an existing spawnset, creating a new one, or closing the application.
- The application window title now displays the current spawnset's name if it has one.
- Added 'File > Save as' menu item.
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
- Performance optimizations, layout improvements, and other bug fixes.

## [2.1.1.0] - 2019-07-05

- Hotfix for restoring the default survival file.

## [2.1.0.0] - 2019-07-05

- Added spawnset sorting to the Download Spawnset window.
- Added Select All and Deselect All buttons to the arena editor.
- Added copy/paste functionality to the spawns editor.
- Added timeout for web requests so the application doesn't keep waiting when the website is offline.
- Fixed tile elements not updating colors after rounding or randomizing.
- The spawns editor scrollbar now scrolls to the end when adding new spawns.
- The Download Spawnset window now remembers the author and spawnset search values after it is closed.
- The default survival file is now embedded into the executable so the actual file is not needed anymore. This removes the issue where the application crashes whenever the file would not be present.
- Optimizations and layout improvements.

## [2.0.0.0] - 2019-06-29

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
  - The application is now dependent on DevilDaggersCore, which is a .NET Standard class library used to share code between various Devil Daggers related applications.

## [1.1.5.0] - 2018-11-04

- Added functionality to automatically check for new versions of the program.

## [1.1.4.0] - 2018-08-05

- Downloading spawnsets and retrieving the spawnset list is now done asynchronously so it doesn't block the application.
- Added functionality to reload the spawnset list if there was no internet connection or if the site was unresponsive.
- Various fixes and small improvements.

## [1.1.2.0] - 2018-07-27

- Added functionality to download spawnsets directly from DevilDaggers.info within the menu.

## [1.0.2.0] - 2018-07-26

- Enforced en-US globalization.

## [1.0.1.0] - 2018-06-25

- Fixed not being able to read some spawnsets made using a hex editor when reading an undefined enemy type.

## [1.0.0.0] - 2018-06-16

- Initial release.

## [0.9.0.0] - 2018-05-14

- Beta release.
