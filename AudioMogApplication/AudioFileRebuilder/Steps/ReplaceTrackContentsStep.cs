using System.IO;
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

			var hcaFilePath = Path.Combine(_hcaFilesFolder, track.ExpectedName);

			if (blackboard.Settings.UseWavFilesIfAvailable)
			{
				var wavFilePath = Path.ChangeExtension(hcaFilePath, ".wav");
				if (File.Exists(wavFilePath))
				{
					blackboard.Logger.Log($"Appending {Path.ChangeExtension(track.ExpectedName, ".wav")}!");
					var wavFileBytes = File.ReadAllBytes(wavFilePath);
					var wavReader = new WaveReader();
					var audioData = wavReader.Read(wavFileBytes);
					var hcaWriter = new HcaWriter();
					var hcaFileBytes = hcaWriter.GetFile(audioData);
					track.HcaPortion = hcaFileBytes;
					return;
				}
			}
			
			if (File.Exists(hcaFilePath))
			{
				blackboard.Logger.Log($"Appending {track.ExpectedName}!");
				var hcaFileBytes = File.ReadAllBytes(hcaFilePath);
				track.HcaPortion = hcaFileBytes;
				return;
			}
			
			blackboard.Logger.Log($"Found no replacement to {track.ExpectedName}, using original track from uexp!");
		}
	}
}