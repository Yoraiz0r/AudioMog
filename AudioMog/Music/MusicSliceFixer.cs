using System;

namespace AudioMog.Core.Music
{
	public class MusicSliceFixer
	{
		public byte[] FileBytes;
		public uint? EntrySample;
		public uint? ExitSample;
		public uint? LoopStartSample;
		public uint? LoopEndSample;
		public void TryReplacing(long offset, uint? value, int addedValue = 0)
		{
			if (!value.HasValue)
				return;
			var sizeBytes = BitConverter.GetBytes(value.Value + addedValue);
			Buffer.BlockCopy(sizeBytes, 0, FileBytes, (int)offset, sizeBytes.Length);
		}
	}
}