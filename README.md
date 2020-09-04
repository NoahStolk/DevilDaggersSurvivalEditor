[![Build Status](https://travis-ci.org/NoahStolk/DevilDaggersSurvivalEditor.svg?branch=master)](https://travis-ci.org/NoahStolk/DevilDaggersSurvivalEditor)

# DevilDaggersSurvivalEditor ([Download](https://devildaggers.info/api/tools/DevilDaggersSurvivalEditor/file))
DevilDaggersSurvivalEditor is a tool that lets you create, view, and edit 'survival' files (also known as spawnsets) for the game Devil Daggers. It is a .NET WPF application built using Visual Studio 2019.

## Framework
.NET Core 3.1

## Language
C# 8.0

## Dependencies
- [DevilDaggersCore](https://github.com/NoahStolk/DevilDaggersCore)

## Main features
- Spawnset editor
- Arena editor with the ability to change tile heights
- Arena presets
- Shrink preview that shows a rough approximation of what the arena will be like at what time during the spawnset
- Easily replacing the currently active spawnset in the game
- Downloading and importing spawnsets directly from [DevilDaggers.info](https://devildaggers.info)

## System requirements
- Microsoft Windows
- [.NET Core 3.1 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.7-windows-x64-installer)

## Installation
1. Download the zip file.
2. Unzip its contents.
3. Run DevilDaggersSurvivalEditor.exe inside the folder.

## Running from source
- Make sure all the projects listed under "Dependencies" are properly referenced. The sources for the projects need to be within the same root folder as the source for DevilDaggersAssetEditor itself.
- DevilDaggersSurvivalEditor uses pixel shaders. In order to build pixel shaders you will need Microsoft's DirectX SDK from June 2010, and use the FXC compiler tool. If you do not need to build pixel shaders (the build for the origin one is already included in the source), then you can skip this step and remove the pre-build event from Build Events in DevilDaggersSurvivalEditor.csproj's properties to prevent it from throwing an error.

## Links
- [DevilDaggers.info website](https://devildaggers.info)
- [Main web page for DevilDaggersSurvivalEditor](https://devildaggers.info/Tools/DevilDaggersSurvivalEditor)
- [List of spawnsets](https://devildaggers.info/Spawnsets)
- [Spawnset guide](https://devildaggers.info/Wiki/SpawnsetGuide)
- [Discord server](https://discord.gg/NF32j8S)

## Credits
While I've built this application myself, I'd like to thank everyone that has put effort into figuring out how the survival file actually works.
[This thread](https://steamcommunity.com/sharedfiles/filedetails/?id=797571917) posted by ThePassiveDada has helped me a lot, as well as [these two editors](https://steamcommunity.com/app/422970/discussions/0/1483232961033779525/) created by bowsr and Sojk.

## License
MIT License

Copyright (c) 2018-2020 Noah Stolk

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
