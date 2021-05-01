using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AudioMog.Application.AudioExtractor;
using AudioMog.Application.AudioFileRebuilder.Steps;
using AudioMog.Core;
using Newtonsoft.Json.Linq;

namespace AudioMog.Application.AudioFileRebuilder
{
	public class AudioRebuilderService : AService
	{
		public const string RebuildSettingsFileName = "RebuildSettings.json";

		public string FilePathForConfig;
		public string RunningDirectory;

		public bool CompareToOriginalFile = false;

		private AudioRebuilderProjectSettings _projectSettings = new AudioRebuilderProjectSettings();
		public string TargetFileName;
		public string ParentDirectory;


		public override void Run()
		{
			Logger.Log($"Beginning to repack {FilePathForConfig}!");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			TryLoadingProjectSettings();

			if (!GetNecessaryPaths())
				return;
			
			if (FindOriginalFile(out var originalFilePath, out var originalExtension))
				return;

			var parser = new FileParser();
			parser.Settings = Settings.Parser;

			var fileBytes = File.ReadAllBytes(originalFilePath);
			var audioBinaryFile = parser.Parse(fileBytes);

			var fileInfo = new FileInfo(originalFilePath);
			var tracks = new List<TemporaryTrack>();
			foreach (var entry in audioBinaryFile.MaterialSection.Entries)
				tracks.Add(new TemporaryTrack()
				{
					OriginalEntry = entry,
					HeaderPortion = fileBytes.SubArray(entry.HeaderPosition, entry.NoHcaHeaderSize),
					HcaPortion = fileBytes.SubArray(entry.HcaStreamStartPosition, entry.HcaStreamSize),
					ExpectedName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}_{entry.EntryIndex:D3}.hca",
				});

			var blackboard = new Blackboard()
			{
				Settings = _projectSettings,
				File = audioBinaryFile,
				Logger = Logger,
				Tracks = tracks,
				FileBytes = fileBytes,
			};
			RunSteps(RunningDirectory, blackboard);

			fileBytes = blackboard.FileBytes;

			List<AudioRebuilderFileOutput> fileOutputs = new List<AudioRebuilderFileOutput>();
			
			var audioFileOutput = new AudioRebuilderFileOutput()
			{
				Extension = originalExtension,
				FileBytes = fileBytes,
			};
			fileOutputs.Add(audioFileOutput);

			GetAdditionalFileOutputsBasedOnFormat(originalFilePath, fileBytes, fileOutputs);

			var outputFileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
			var outputFolder = RunningDirectory;
			WriteFilesTo(outputFolder, outputFileName, fileOutputs);

			var additionalPaths = _projectSettings?.AdditionalOutputFolders ?? new string[0];
			foreach (var additionalPath in additionalPaths)
			{
				if (!Directory.Exists(additionalPath))
				{
					Logger.Warn($"Could not write files to additional output path: {additionalPath}, because it doesn't exist!");
					continue;
				}
				WriteFilesTo(additionalPath, outputFileName, fileOutputs);
			}
			
			stopwatch.Stop();
			Logger.Log($"Done repacking {FilePathForConfig}! (total time: {stopwatch.Elapsed:s\\.fff}s)");
		}

		private bool FindOriginalFile(out string expectedFilePath, out string originalExtension)
		{
			expectedFilePath = null;
			originalExtension = null;
			foreach (var extension in AudioExtractorService.AcceptedExtensions)
			{
				if (!TryGettingFileByExtension(TargetFileName, extension, out var originalFilePath))
					continue;

				expectedFilePath = originalFilePath;
				originalExtension = extension;
				break;
			}

			if (expectedFilePath == null)
			{
				Logger.Error($"Failed to find project's original file! Looked for {TargetFileName} at: {ParentDirectory}");
				return true;
			}

			return false;
		}

		private void GetAdditionalFileOutputsBasedOnFormat(string filePath, byte[] fileBytes, List<AudioRebuilderFileOutput> fileOutputs)
		{
			if (Path.GetExtension(filePath).ToLower() == ".uexp")
			{
				if (TryGettingFileByExtension(TargetFileName, ".uasset", out var uAssetFilePath))
				{
					var uassetBytes = File.ReadAllBytes(uAssetFilePath);
					var sizeBytes = BitConverter.GetBytes((uint) (fileBytes.Length - 4));
					Buffer.BlockCopy(
						sizeBytes, 0, fileBytes,
						uassetBytes.Length - Settings.Parser.UAssetFileSizeOffsetFromEndOfFile,
						sizeBytes.Length);

					var uassetFileOutput = new AudioRebuilderFileOutput()
					{
						Extension = ".uasset",
						FileBytes = uassetBytes,
					};
					fileOutputs.Add(uassetFileOutput);
				}
				else
					Logger.Error(
						$"Failed to find project's original uasset! Looked for {TargetFileName} at: {ParentDirectory}");
			}
		}

		private bool GetNecessaryPaths()
		{
			RunningDirectory = Path.GetDirectoryName(FilePathForConfig);
			var folderSuffix = AudioExtractorService.ExtractionSuffix;
			if (!RunningDirectory.EndsWith(folderSuffix))
			{
				Logger.Error(
					$"The folder name does not end with {folderSuffix}, which prevents finding the original asset to modify!");
				return false;
			}

			var parentFolderInfo = Directory.GetParent(RunningDirectory);
			ParentDirectory = parentFolderInfo.FullName;
			TargetFileName = RunningDirectory.Substring(0, RunningDirectory.LastIndexOf(folderSuffix, StringComparison.Ordinal));
			return true;
		}

		private void TryLoadingProjectSettings()
		{
			var filePath = FilePathForConfig;
			try
			{
				var fileText = File.ReadAllText(filePath);
				var json = JObject.Parse(fileText);
				var sessionSettings = json.ToObject<AudioRebuilderProjectSettings>();
				_projectSettings = sessionSettings;
			}
			catch (Exception)
			{
				Logger.Warn("Failed to load audio rebuilder settings file! will use default settings, instead!");
			}
		}

		private void WriteFilesTo(string outputFolder, string outputFileName, List<AudioRebuilderFileOutput> fileOutputs)
		{
			foreach (var fileOutput in fileOutputs)
			{
				var fileOutputPath = Path.Combine(outputFolder, Path.ChangeExtension(outputFileName, fileOutput.Extension));
				try
				{
					File.WriteAllBytes(fileOutputPath, fileOutput.FileBytes);
					Logger.Log($"Created repacked uexp at: {fileOutputPath}");
				}
				catch (Exception e)
				{
					Logger.Error($"Encountered an error trying to write the new files! {e}");
					break;
				}
			}
		}

		private void RunSteps(string hcaFilesFolder, Blackboard blackboard)
		{
			var fileBytes = blackboard.FileBytes;
			var originalBackup = new byte[fileBytes.Length];
			Array.Copy(fileBytes, originalBackup, originalBackup.Length);

			var stepsBeforeRebuildingFile = new List<ARebuilderStep>()
			{
				new ReplaceTrackContentsStep(hcaFilesFolder),
				new FixTrackHeadersStep(),
			};

			var stepsAfterRebuildingFile = new List<ARebuilderStep>()
			{
				new RebuildFileBytesFromFixedTracksStep(),
				new FixMaterialTrackOffsetsStep(),
				new FixMusicSlicesStep(),
				new FixTotalFileSizeStep(),
			};

			if (CompareToOriginalFile)
				for (int i = stepsAfterRebuildingFile.Count - 1; i >= 0; i--)
					stepsAfterRebuildingFile.Insert(i, new CompareToOriginalStep(originalBackup));

			var steps = new List<ARebuilderStep>();
			steps.AddRange(stepsBeforeRebuildingFile);
			steps.AddRange(stepsAfterRebuildingFile);

			foreach (var step in steps)
				step.Run(blackboard);
			
			
		}

		private bool TryGettingFileByExtension(string targetFileName, string targetFileExtension, out string targetFilePath)
		{
			var targetFileNameWithExtension = $"{targetFileName}{targetFileExtension}";
			targetFilePath = Path.Combine(ParentDirectory, targetFileNameWithExtension);
			if (!File.Exists(targetFilePath))
				return false;
			return true;
		}
	}
}