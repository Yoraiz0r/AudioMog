using System;
using System.IO;
using System.Linq;
using AudioMog.Application;
using AudioMog.Application.AudioExtractor;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Core;
using AudioMog.Core.Audio;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AudioMog.Terminal
{
	internal class Program
	{
		public const string AssemblyVersion = "2021.06.05.02";
		public const string AssemblyFileVersion = "2021.06.05.02";
		
		private static ServiceProvider _serviceProvider;
		public static ConsoleApplicationLogger Logger;
		public static ProgramSettings Settings;
		public static string ApplicationPath;

		private const string ConfigFilePath = "TerminalSettings.json";

		public static ProgramSettings DefaultSettings = new ProgramSettings()
		{
			TerminalSettings = new TerminalSettings()
			{
				ImmediatelyQuitOnceAllTasksAreDone = false,
				LogLevel = 3,
			},
			ApplicationSettings = new ApplicationSettings()
			{
				Parser = new FileParserSettings()
				{
					SoundFileFileVersion = new AudioBinaryFileVersion() {Main = 2, Sub = 0},
					MusicFileFileVersion = new AudioBinaryFileVersion() {Main = 2, Sub = 1},
				},
				AudioExtractor = new AudioExtractorSettings()
				{
					ExtractAsRaw = false,
					ExtractAsWav = true,
				}
			},
		};

		public static void Main(string[] args)
		{
			Logger = new ConsoleApplicationLogger();
			ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

			ShowGreeting();
			
			Settings = DefaultSettings;
			TryLoadingSettings();

			_serviceProvider = new ServiceProvider(Logger, Settings.ApplicationSettings);

			if (args.Length == 0)
				ShowUsageInstructions();

			var shouldWaitForInputOnceDone =
				!Settings.TerminalSettings.ImmediatelyQuitOnceAllTasksAreDone || args.Length == 0;
			Logger.LogLevel = (ProgramLogLevel)Settings.TerminalSettings.LogLevel;
			foreach (var arg in args)
			{
				try
				{
					var strategy = GetHandleStrategyFor(arg);
					if (strategy == null)
						continue;

					strategy.Run();
				}
				catch (Exception e)
				{
					Logger.Error($"{e}");
					shouldWaitForInputOnceDone = true;
				}
				Logger.Log("");
			}
			Logger.LogLevel = ProgramLogLevel.Everything;

			if (shouldWaitForInputOnceDone)
				WaitForInput();
		}

		private static void ShowGreeting()
		{
			Logger.Log($"Running AudioMog v{AssemblyVersion}!");
			Logger.Log("");
		}

		private static void TryLoadingSettings()
		{
			var filePath = Path.GetFullPath(Path.Combine(ApplicationPath, ConfigFilePath));
			if (!File.Exists(filePath))
			{
				var configText = JsonConvert.SerializeObject(DefaultSettings, Formatting.Indented);
				var outputPath = Path.Combine(ApplicationPath, ConfigFilePath);
				File.WriteAllText(outputPath, configText);
				return;
			}

			var fileText = File.ReadAllText(filePath);
			try
			{
				var json = JObject.Parse(fileText);
				var settings = json.ToObject<ProgramSettings>();
				Settings = settings;
			}
			catch (JsonReaderException)
			{
				Logger.Error("Failed to load application settings file! will use default settings, instead!");
			}
		}

		private static void ShowUsageInstructions()
		{
			Logger.Log("Hello there! This is AudioMog, an all-in-one tool to mod audio files for several games!");
			Logger.Log($"AudioMog was written for, and only tested with, Kingdom Hearts III, but should work for other Square Enix games!");
			Logger.Log("");
			Logger.Log("In order to use AudioMog, you have to drag supported files onto the executable (AudioMog.exe), or nothing will happen!");
			Logger.Log($"The following file types are accepted: .json|{string.Join("|", AudioExtractorService.AcceptedExtensions)}.");
			Logger.Log("");
			Logger.Log("Here is how AudioMog handles each respective file type:");
			Logger.Log($"{string.Join("|", AudioExtractorService.AcceptedExtensions)}: Unpack usable audio within the file into a folder, and creates a repacking project.");
			Logger.Log(".json: Build a new audio binary file, with any compatible audio file overrides, in its running folder.");
			Logger.Log("");
			Logger.Log("AudioMog comes with a TerminalSettings.json file. Use it to customize AudioMog for other Square Enix games!");
			Logger.Log("");
			Logger.Log("AudioMog was written by Yoraiz0r");
			Logger.Log("Lots of help thanks to prior research of vgmstream's contributors!");
			Logger.Log("Special thanks to: Ray Cooper, Normie, ShonicTH, and the entire OpenKH community!");
			Logger.Log("");
		}


		private static AService GetHandleStrategyFor(string argument)
		{
			if (File.Exists(argument))
			{
				var fileInfo = new FileInfo(argument);
				var extension = fileInfo.Extension.ToLower();
				
				if (AudioExtractorService.AcceptedExtensions.Contains(extension))
				{
					var service = _serviceProvider.GetService<AudioExtractorService>();
					service.FilePathToExtract = argument;
					return service;
				}

				if (extension == ".json")
				{
					var service = _serviceProvider.GetService<AudioRebuilderService>();
					service.FilePathForConfig = argument;
					return service;
				}
			}

			return null;
		}

		private static void WaitForInput()
		{
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}