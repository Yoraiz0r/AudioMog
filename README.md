# AudioMog
AudioMog is a free all-in-one audio modding tool, that allows users to unpack, and repack supported game's audio binary files.
AudioMog was created to assist Kingdom Hearts III modding endeavors, and works on other games such as Melody of Memory, Final Fantasy XV, and more.


# Features
- Unpack audio binary files for supported games into playable wav/hca files
- Repack audio binary files with overrides to specific tracks, such that games will be able to use them properly.
- Provide human-readable track names, for an easier modding experience


# Supported formats
Format | Unpack | Repack | Notes
--- | --- | --- | ---
.uexp | ✓ | ✓ | Repacking needs an additional .uasset file
.bytes | ✓ | ✓ | 
.sab | ✓ | ✓ | 
.sabf | ✓ | ✓ | 
.mab | ✓ | ✓ |
.mabf | ✓ | ✓ |


# Usage
In order to use AudioMog, you have to drag supported files onto the executable (AudioMog.exe), or nothing will happen! 
1. Drag any file of the supported formats above to extract its contents, and create an AudioMog project settings .json file.
2. Replace/override audio files that you wish to change. (e.g. replace `music_001.wav` with a new `music_001.wav`)
3. (Optional) Customize the project settings .json file to your liking
4. Drag the generated project settings .json file to repack the audio back to its original format
5. Load the repacked audio binary file into the game using whatever the game's preferred mod loading method is.


# Terminal Settings
AudioMog holds several settings under `TerminalSettings.json`, that you must adjust for specific games to work.
* **Parser**
  * `UAssetFileSizeOffsetFromEndOfFile`: UAsset files file size, and each game holds that at a different offset.
  * `MusicFileVersion`: When reading music audio binary files, you can only read files of a matching version to this.
  * `SoundFileVersion`: When reading sound audio binary files, you can only read files of a matching version to this.
* **Audio Extractor**
  * `ExtractAsHca`: Internally, all supported games use `.hca` files, set this to `true` to extract these files as they are.
  * `ExtractAsWav`: Should audio files be converted to `.wav` upon extraction?
  * You can set both `ExtractAsHca` and `ExtractAsWav` to `true`, which will give you the file in both formats.
* **Terminal Settings**
  * `ImmediatelyQuitOnceAllTasksAreDone` set this to true to automatically close the AudioMog terminal once tasks are finished.
  * `LogLevel` Which messages should AudioMog's terminal show? (0: Nothing, 1: Errors only, 2: Errors & Warnings, 3: Everything)


# Project Settings
Whenever you extract audio files using AudioMog, you also get a `RebuildSettings.json` file.
Customizing `RebuildSettings.json` lets you control the repacking in a tighter manner.
- `UseWavFilesIfAvailable`: When set to `true`, AudioMog accepts `.wav` & `.hca` files for rebuilding, instead of just `.hca`. (and prefers `.wav` first)
- `AdditionalOutputFolders`: Specify folder paths here if you want AudioMog to output rebuilt files to these paths, too. (such as directly into your mod pak paths)
- `Originals`: This is exclusive to music files, and is not used by AudioMog. It lets you see see the default values for music slices, so that you can copy and edit them in the `Overrides` section.
- `Overrides`: If you wish to override a music slice's entry and exit samples, copy the original slice from `Originals` in here, and edit the sample values.  
  
  
# Music Slice Fix Settings
If you're editing music files, here are the fields you can put inside each entry of the `Overrides` array.
- `LoopStartSample`: (optional) What sample does the music loop start at.
- `LoopEndSample`: (optional) What sample does the music loop end at.
- `EntrySample`: (optionial) What sample does the music file start at.
- `ExitSample`: (optional) What sample does the music file end at.
- All of these fields are entirely optional, but **if you are replacing music files with different sample rates or lengths, you may need to mess with these.**


# Requirements
- [.Net Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)


# Build
- The solution's projects use some C# 7 syntax, and as such require Visual Studio 2017 (or newer)


# Credits
- AudioMog is written by Yoraiz0r
- Most of the knowledge applied in AudioMog's code comes from [vgmstream](https://github.com/vgmstream/vgmstream)
- Special thanks to Ray Cooper, normie, ShonicTH, and the entire OpenKH community!
