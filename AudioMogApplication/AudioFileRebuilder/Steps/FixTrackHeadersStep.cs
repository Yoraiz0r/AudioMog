namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class FixTrackHeadersStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			foreach (var track in blackboard.Tracks)
				FixHeader(track);
		}
			
		private void FixHeader(TemporaryTrack track)
		{
			ExposedHcaReader reader = new ExposedHcaReader();
			var data = reader.GetStructure(track.HcaPortion);
			
			var hcaHeaderSize = data.HeaderSize;
			
			int loopStart = 0;
			int loopEnd = 0;
			if (data.Hca.Looping)
			{
				loopStart = data.Hca.LoopStartSample;
				loopEnd = data.Hca.LoopEndSample;
			}
			
			var newExtraDataSize = track.OriginalEntry.NoHcaHeaderExtraDataSize + data.HeaderSize;
			var streamSize = track.HcaPortion.Length - hcaHeaderSize;
			
			var headerBytes = track.HeaderPortion;
			WriteByte(headerBytes, 0x04, (byte)data.Hca.ChannelCount);
			WriteUint(headerBytes, 0x08, (uint)data.Hca.SampleRate);
			WriteUint(headerBytes, 0x0c, (uint)loopStart);
			WriteUint(headerBytes, 0x10, (uint)loopEnd);
			WriteUint(headerBytes, 0x14, (uint)newExtraDataSize);
			WriteUint(headerBytes, 0x18, (uint)streamSize);

			var extraData = 0x20;
			WriteByte(headerBytes, extraData + 0x00, 1);
			//WriteByte(headerBytes, extraData + 0x01, 16); //extraDataSize
			WriteUshort(headerBytes, extraData + 0x02, (ushort)data.HeaderSize);
			WriteUshort(headerBytes, extraData + 0x04, (ushort)data.Hca.FrameSize);
			WriteUshort(headerBytes, extraData + 0x06, (ushort)data.Hca.LoopStartFrame);
			WriteUshort(headerBytes, extraData + 0x08, (ushort)data.Hca.LoopEndFrame);
			WriteUshort(headerBytes, extraData + 0x0a, (ushort)data.Hca.InsertedSamples);
			// 0x0c: "use mixer" flag
			// 0x0d: encryption flag 
			// 0x0e: reserved x2 
		}
	}
}