using AudioMog.Core.Audio;

namespace AudioMog.Application.Codecs
{
	public static class AvailableCodecs
	{
		public static ACodec GetCodec(MaterialCodecType codec)
		{
			switch (codec)
			{
				default: return null;
				case MaterialCodecType.OGGVorbis: return new OggVorbisCodec();
				case MaterialCodecType.HCA: return new HcaACodec();
			}
		}
	}
}