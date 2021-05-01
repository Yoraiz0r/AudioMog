using AudioMog.Core.Audio;

namespace AudioMog.Application.AudioFileRebuilder
{
	public class TemporaryTrack
	{
		public byte[] HeaderPortion;
		public byte[] HcaPortion;
		public string ExpectedName;
		public MaterialSection.MaterialEntry OriginalEntry;
	}

}