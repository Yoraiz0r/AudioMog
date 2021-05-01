using System.Collections.Generic;
using System.IO;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class FixMaterialTrackOffsetsStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			FixMTRLTrackOffsets(blackboard.Tracks, blackboard.FileBytes);
		}
		private void FixMTRLTrackOffsets(List<TemporaryTrack> tracks, byte[] fileBytes)
		{
			uint accumulatedOffset = tracks[0].OriginalEntry.LocalSectionOffset;
			var memoryStream = new MemoryStream(fileBytes);
			var binaryWriter = new BinaryWriter(memoryStream);

			for (int i = 0; i < tracks.Count; i++)
			{
				var track = tracks[i];

				var positionToWriteTo = track.OriginalEntry.PositionOfOffsetFromMtrlSectionOffset;
				binaryWriter.BaseStream.Position = positionToWriteTo;
				binaryWriter.Write(accumulatedOffset);
				
				//Debug.WriteLine($"entry: {i}, accumulated: {accumulatedOffset}, MTRLOffset: {yoraiInfo.OffsetFromWeirdPoint}, trackSize: {trackSize}");
				accumulatedOffset += (uint)track.HcaPortion.Length + (uint)track.HeaderPortion.Length;
				var hexAignmentRemainder = (track.HcaPortion.Length + track.HeaderPortion.Length) % 16;
				var valueToFixAlignment = (16 - hexAignmentRemainder) % 16;
				accumulatedOffset += (uint)valueToFixAlignment;
			}
		}
	}
}