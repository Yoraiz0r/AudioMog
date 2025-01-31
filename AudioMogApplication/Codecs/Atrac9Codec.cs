using System.IO;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Core;
using AudioMog.Core.Audio;
using VGAudio.Containers.At9;
using VGAudio.Containers.Wave;

namespace AudioMog.Application.Codecs
{
	public class Atrac9Codec : ACodec
	{
		public override string FileFormat => ".atrac9";
		public override void ExtractOriginal(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string outputPath)
		{
			var rawContentBytes = fullFileBytes.SubArray(entry.InnerStreamStartPosition, entry.InnerStreamSize);
			
			var rawPath = ExtensionMethods.ChangeExtension(outputPath, FileFormat);
			File.WriteAllBytes(rawPath, rawContentBytes);
			logger.Log($"Created atrac9 audio track at: {rawPath}");
		}

		public override void ExtractAsWav(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string wavPath)
		{
			var rawContentBytes = fullFileBytes.SubArray(entry.InnerStreamStartPosition, entry.InnerStreamSize);

			var reader = new At9Reader();
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