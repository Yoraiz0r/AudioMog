using System;

namespace AudioMog.Application.AudioFileRebuilder
{
	[Serializable]
	public class MusicTrackFixObject
	{
		public int MusicIndex;
		public int SliceIndex;
		public uint[] MaterialsUsed;
		public uint? LoopStartSample;
		public uint? LoopEndSample;
		public uint? EntrySample;
		public uint? ExitSample;
	}
}