using System;

namespace AudioMog.Application.AudioFileRebuilder
{
	[Serializable]
	public class AudioRebuilderProjectSettings
	{
		public bool UseWavFilesIfAvailable;
		public string[] AdditionalOutputFolders;
		public MusicTrackFixObject[] Overrides;
		public MusicTrackFixObject[] Originals;
	}
}