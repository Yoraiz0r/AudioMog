using System.IO;
using AudioMog.Application.Codecs;
using AudioMog.Core.Audio;
using VGAudio.Containers.Hca;
using VGAudio.Containers.Wave;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class ReplaceTrackContentsStep : ARebuilderStep
	{
		private readonly string _hcaFilesFolder;

		public ReplaceTrackContentsStep(string hcaFilesFolder)
		{
			_hcaFilesFolder = hcaFilesFolder;
		}
		public override void Run(Blackboard blackboard)
		{
			foreach (var track in blackboard.Tracks)
				TryReplacingTrackContents(blackboard, track);
		}
		private void TryReplacingTrackContents(Blackboard blackboard, TemporaryTrack track)
		{
			if (track.ExpectedName == null)
				return;

			var originalCodec = AvailableCodecs.GetCodec(track.CurrentCodec);
			if (originalCodec == null)
			{
				blackboard.Logger.Error($"The track ({track.ExpectedName}) uses the {track.OriginalEntry.Codec} Codec, but AudioMog has no handler to replace it! Skipping!");
				return;
			}
			
			var typelessFilePath = Path.Combine(_hcaFilesFolder, track.ExpectedName);

			if (blackboard.Settings.UseWavFilesIfAvailable)
			{
				var wavFilePath = Path.ChangeExtension(typelessFilePath, ".wav");
				if (File.Exists(wavFilePath))
				{
					blackboard.Logger.Log($"Appending {Path.ChangeExtension(track.ExpectedName, ".wav")}!");
					var wavFileBytes = File.ReadAllBytes(wavFilePath);
					var wavReader = new WaveReader();
					var audioData = wavReader.Read(wavFileBytes);
					var hcaWriter = new HcaWriter();
					var hcaFileBytes = hcaWriter.GetFile(audioData);
					track.RawPortion = hcaFileBytes;
					track.CurrentCodec = MaterialCodecType.HCA;
					return;
				}
			}


			var hcaFilePath = Path.ChangeExtension(typelessFilePath, ".hca");
			if (File.Exists(hcaFilePath))
			{
				blackboard.Logger.Log($"Appending {Path.ChangeExtension(track.ExpectedName, ".hca")}!");
				var hcaFileBytes = File.ReadAllBytes(hcaFilePath);
				track.RawPortion = hcaFileBytes;
				track.CurrentCodec = MaterialCodecType.HCA;
				return;
			}
			
			var rawFilePath = Path.ChangeExtension(typelessFilePath, originalCodec.FileFormat);
			if (File.Exists(rawFilePath))
			{
				blackboard.Logger.Log($"Appending {Path.ChangeExtension(track.ExpectedName, originalCodec.FileFormat)}!");
				var hcaFileBytes = File.ReadAllBytes(rawFilePath);
				track.RawPortion = hcaFileBytes;
				track.CurrentCodec = track.OriginalEntry.Codec;
				return;
			}
			
			blackboard.Logger.Log($"Found no replacement to {track.ExpectedName}, using original track from uexp!");
		}
	}
}