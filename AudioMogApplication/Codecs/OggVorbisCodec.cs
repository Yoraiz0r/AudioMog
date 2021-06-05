using System.IO;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Application.Utilities;
using AudioMog.Core;
using AudioMog.Core.Audio;
using NVorbis;
using NVorbis.Contracts;
using StbVorbisSharp;

namespace AudioMog.Application.Codecs
{
	public class OggVorbisCodec : ACodec
	{
		public override string FileFormat => ".ogg"; 

		public override void ExtractOriginal(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string outputPath)
		{
			var rawBytes = fullFileBytes.SubArray(entry.InnerStreamStartPosition, entry.InnerStreamSize);

			var rawPath = ExtensionMethods.ChangeExtension(outputPath, FileFormat);
			File.WriteAllBytes(rawPath, rawBytes);
			logger.Log($"Created ogg audio track at: {rawPath}");
		}

		public override void ExtractAsWav(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string wavPath)
		{
			var rawContentBytes = fullFileBytes.SubArray(entry.InnerStreamStartPosition, entry.InnerStreamSize);
			var vorbis = Vorbis.FromMemory(rawContentBytes);


			WavWriterLoopPoint[] loopPoints = null;
			if (entry.IsLooping)
				loopPoints = new[] { new WavWriterLoopPoint { StartSample = entry.LoopStart, EndSample = entry.LoopEnd }};
			
			var wavWriter = new CustomWavWriter();
			wavWriter.WriteWav(new CustomWavWriterRequest()
			{
				WavPath = wavPath,
				Channels = vorbis.Channels,
				SampleRate = vorbis.SampleRate,
				WavSampleWriter = new OggVorbisToWavSampleWriter(vorbis),
				LoopPoints = loopPoints,
			});

			logger.Log($"Created wav audio track at: {wavPath}");
		}

		public override void FixHeader(TemporaryTrack track)
		{
			using (var stream = new MemoryStream(track.RawPortion))
			using (var reader = new VorbisReader(stream))
			{
				int loopStart = 0;
				int loopEnd = 0;

				var tags = reader.Tags;
				TryGettingVariable(tags, "LOOPSTART", ref loopStart);
				TryGettingVariable(tags, "LOOPEND", ref loopEnd);

				var headerSize = 0;
				var newExtraDataSize = track.OriginalEntry.NoStreamHeaderExtraDataSize + headerSize;
				var streamSize = track.RawPortion.Length - headerSize;

				var headerBytes = track.HeaderPortion;
				WriteByte(headerBytes, 0x04, (byte) reader.Channels);
				WriteUint(headerBytes, 0x08, (uint) reader.SampleRate);
				WriteUint(headerBytes, 0x0c, (uint) loopStart);
				WriteUint(headerBytes, 0x10, (uint) loopEnd);
				WriteUint(headerBytes, 0x14, (uint) newExtraDataSize);
				WriteUint(headerBytes, 0x18, (uint) streamSize);
			}
		}

		private void TryGettingVariable(ITagData tags, string variableWeLookFor, ref int variableValueHolder)
		{
			var vorbisComment = tags.GetTagSingle(variableWeLookFor);
			if (!vorbisComment.StartsWith(variableWeLookFor))
				return;

			int value;
			if (int.TryParse(vorbisComment, out value))
				variableValueHolder = value;
		}
	}
}