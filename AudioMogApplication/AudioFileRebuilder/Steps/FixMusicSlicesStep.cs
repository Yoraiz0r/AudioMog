using AudioMog.Core.Audio;
using AudioMog.Core.Music;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class FixMusicSlicesStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			TryFixingMusicSlices(blackboard.Settings, blackboard.File, blackboard.FileBytes);
		}
		
		private void TryFixingMusicSlices(AudioRebuilderProjectSettings settings, AAudioBinaryFile file, byte[] fileBytes)
		{
			if (!(file is MusicAudioBinaryFile mab))
				return;

			foreach (var fix in settings.Overrides ?? new MusicTrackFixObject[0])
			{
				var music = mab.Entries[fix.MusicIndex];
				var slice = music.Slices[fix.SliceIndex];
				
				var fixInfo = new MusicSliceFixer()
				{
					FileBytes = fileBytes,
					LoopStartSample = fix.LoopStartSample,
					LoopEndSample = fix.LoopEndSample,
					EntrySample = fix.EntrySample,
					ExitSample = fix.ExitSample
				};
				slice.RunFixes(fixInfo);
			}
		}
	}
}