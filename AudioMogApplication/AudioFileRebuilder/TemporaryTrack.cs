using AudioMog.Core.Audio;

namespace AudioMog.Application.AudioFileRebuilder
{
	public class TemporaryTrack
	{
		public byte[] HeaderPortion;
		public byte[] RawPortion;
		public string ExpectedName;
		public MaterialCodecType CurrentCodec;
		public MaterialSection.MaterialEntry OriginalEntry;
	}

}