using System;
using AudioMog.Core.Audio;

namespace AudioMog.Core
{
	[Serializable]
	public class FileParserSettings
	{
		public AudioBinaryFileVersion MusicFileFileVersion;
		public AudioBinaryFileVersion SoundFileFileVersion;
		public int? OverrideUAssetFileSizeOffsetFromEndOfFile;
	}
}