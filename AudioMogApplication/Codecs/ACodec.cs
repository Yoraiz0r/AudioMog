using System;
using AudioMog.Application.AudioFileRebuilder;
using AudioMog.Core.Audio;

namespace AudioMog.Application.Codecs
{
	public abstract class ACodec
	{
		public abstract string FileFormat { get; }

		public abstract void ExtractOriginal(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string outputPath);
		
		public abstract void ExtractAsWav(IApplicationLogger logger, MaterialSection.MaterialEntry entry, byte[] fullFileBytes, string wavPath);

		public abstract void FixHeader(TemporaryTrack track);
		
		
		protected void WriteUint(byte[] bytes, int offset, uint value)
		{
			var sizeBytes = BitConverter.GetBytes(value);
			Buffer.BlockCopy(sizeBytes, 0, bytes, offset, sizeBytes.Length);
		}
		protected void WriteUshort(byte[] bytes, int offset, ushort value)
		{
			var sizeBytes = BitConverter.GetBytes(value);
			Buffer.BlockCopy(sizeBytes, 0, bytes, offset, sizeBytes.Length);
		}
		protected void WriteByte(byte[] bytes, int offset, byte value)
		{
			bytes[offset] = value;
		}
	}
}