using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Core;
using AudioMog.Core.Audio;
using AudioMog.Core.Music;
using Newtonsoft.Json;
using VGAudio.Containers.Hca;
using VGAudio.Containers.Wave;

namespace AudioMog.Application.AudioExtractor
{
	public class AudioExtractorService : AService
	{
		public const string ExtractionSuffix = "_Project";
		public const string TrackNamesFileName = "TrackUsers.txt";
		public static readonly string[] AcceptedExtensions = {".uexp", ".bytes", ".sab", ".sabf", ".mab", ".mabf"};

		public string FilePathToExtract;
		private Dictionary<int, string> _materialIndexToFileName = new Dictionary<int, string>();
		private Dictionary<int, string> _materialIndexToUserNames = new Dictionary<int, string>();

		public override void Run()
		{
			Logger.Log($"Beginning to unpack {FilePathToExtract}!");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var fileBytes = File.ReadAllBytes(FilePathToExtract);

			var parser = new FileParser();
			parser.Settings = Settings.Parser;

			var audioBinaryFile = parser.Parse(fileBytes);
			var materialEntries = audioBinaryFile.MaterialSection.Entries;

			var actualFileName = Path.GetFileNameWithoutExtension(FilePathToExtract);
			var outputFolder = Path.Combine(Path.GetDirectoryName(FilePathToExtract),
				$"{actualFileName}{ExtractionSuffix}");

			CollectMaterialToFileNames(audioBinaryFile, actualFileName);
			CollectMaterialToUserNames(audioBinaryFile);

			WriteOutAudioFiles(audioBinaryFile, fileBytes, outputFolder);
			
			var humaneNamesFileText = BuildHumanNamesFileText(materialEntries);
			var humaneNamesFilePath = Path.Combine(outputFolder, TrackNamesFileName);
			File.WriteAllText(humaneNamesFilePath, humaneNamesFileText);
			Logger.Log($"Created track users documentation at: {humaneNamesFilePath}");

			WriteProjectFile(audioBinaryFile, outputFolder);

			stopwatch.Stop();
			Logger.Log($"Done unpacking {FilePathToExtract}! (total time: {stopwatch.Elapsed:s\\.fff}s)");
		}

		private void WriteProjectFile(AAudioBinaryFile file, string outputFolder)
		{
			var config = new JsonSerializerSettings();
			config.NullValueHandling = NullValueHandling.Ignore;
			var projectSettings = new AudioRebuilderProjectSettings()
			{
				UseWavFilesIfAvailable = true,
				AdditionalOutputFolders = new string[0],
				Overrides = new MusicTrackFixObject[0],
			};

			if (file is MusicAudioBinaryFile mabf)
			{
				config.NullValueHandling = NullValueHandling.Include;
				List<MusicTrackFixObject> originals = new List<MusicTrackFixObject>();
				
				foreach (var entry in mabf.Entries)
				foreach (var slice in entry.Slices)
				{
					var fixObject = new MusicTrackFixObject()
					{
						MusicIndex = entry.Index,
						SliceIndex = slice.Index,
						MaterialsUsed = slice.Layers.Select(x => (uint)x.MaterialIndex).ToArray(),
						EntrySample = slice.EntryPointsSample,
						ExitSample = slice.ExitPointsSample
					};
					originals.Add(fixObject);
				}

				projectSettings.Originals = originals.ToArray();
			}
			

			var projectFilePath = Path.Combine(outputFolder, AudioRebuilderService.RebuildSettingsFileName);
			string projectSettingsText = JsonConvert.SerializeObject(projectSettings, Formatting.Indented, config);
			File.WriteAllText(projectFilePath, projectSettingsText);
			Logger.Log($"Created project settings at: {projectFilePath}");
		}

		private void WriteOutAudioFiles(AAudioBinaryFile audioBinaryFile, byte[] fileBytes, string outputFolder)
		{
			var materialEntries = audioBinaryFile.MaterialSection.Entries;
			foreach (var entry in materialEntries)
			{
				var hcaFileBytes = fileBytes.SubArray(entry.HcaStreamStartPosition, entry.HcaStreamSize);

				var extractedFilePath = Path.Combine(outputFolder, _materialIndexToFileName[entry.EntryIndex]);

				var fullFilePath = Path.GetFullPath(extractedFilePath);
				var fullFolderPath = Path.GetDirectoryName(fullFilePath);

				if (!Directory.Exists(fullFolderPath))
				{
					Directory.CreateDirectory(fullFolderPath);
					Logger.Log($"Created folder at: {fullFolderPath}");
				}

				if (Settings.AudioExtractor.ExtractAsHca)
				{
					File.WriteAllBytes(fullFilePath, hcaFileBytes);
					Logger.Log($"Created hca audio track at: {fullFilePath}");
				}

				if (Settings.AudioExtractor.ExtractAsWav)
				{
					HcaReader reader = new HcaReader();
					var audioData = reader.Read(hcaFileBytes);
					
					WaveWriter writer = new WaveWriter();
					var wavBytes = writer.GetFile(audioData);

					var wavPath = Path.ChangeExtension(fullFilePath, ".wav");
					File.WriteAllBytes(wavPath, wavBytes);
					Logger.Log($"Created wav audio track at: {wavPath}");
				}
			}
		}

		private void CollectMaterialToFileNames(AAudioBinaryFile audioBinaryFile, string actualFileName)
		{
			var materialEntries = audioBinaryFile.MaterialSection.Entries;
			foreach (var entry in materialEntries)
			{
				var extractedFileName = $"{actualFileName}_{entry.EntryIndex:D3}.hca";
				_materialIndexToFileName[entry.EntryIndex] = extractedFileName;
			}
		}

		private void CollectMaterialToUserNames(AAudioBinaryFile audioBinaryFile)
		{
			var materialEntries = audioBinaryFile.MaterialSection.Entries;
			var users = audioBinaryFile.MaterialSection.Users;
			
			foreach (var entry in materialEntries)
			{
				var entryUsers = users
					.Where(user => user.MaterialIndex == entry.EntryIndex)
					.Select(user => user.User.DisplayName);
				var userNames = string.Join(", ", entryUsers);
				_materialIndexToUserNames.Add(entry.EntryIndex, userNames);
			}
		}

		private string BuildHumanNamesFileText(List<MaterialSection.MaterialEntry> materialEntries)
		{
			var stringBuilder = new StringBuilder();
			foreach (var entry in materialEntries)
			{
				var entryFileName = _materialIndexToFileName[entry.EntryIndex];
				var entryUsers = _materialIndexToUserNames[entry.EntryIndex];
				stringBuilder.AppendLine($"{entryFileName},		users: {entryUsers}"); //
			}

			return stringBuilder.ToString();
		}
	}
}