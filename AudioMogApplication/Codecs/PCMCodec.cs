using System.IO;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Core;
using AudioMog.Core.Audio;
using VGAudio.Containers.Wave;
using VGAudio.Formats;

namespace AudioMog.Application.Codecs
{
	public class PCMCodec : ACodec
	{
		public override string FileFormat => ".pcm"; 
		public override void ExtractOriginal(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string outputPath)
		{
			ExtractAsWav(logger, entry, fullFileBytes, outputPath);
		}

		public override void ExtractAsWav(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string wavPath)
		{
			var rawContentBytes = fullFileBytes.SubArray(entry.InnerStreamStartPosition, entry.InnerStreamSize);

			WaveReader reader = new WaveReader();
			var audioData = reader.Read(rawContentBytes);
			
			WaveWriter writer = new WaveWriter();
			var wavBytes = writer.GetFile(audioData);

			File.WriteAllBytes(wavPath, wavBytes);
			logger.Log($"Created wav audio track at: {wavPath}");
		}

		public override void FixHeader(TemporaryTrack track)
		{
		}
	}
}