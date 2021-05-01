using System.Collections.Generic;
using System.Linq;
using AudioMog.Core;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class RebuildFileBytesFromFixedTracksStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			var original = blackboard.File;

			var innerFileUpToFirstTrack = original.InnerFileBytes
				.SubArray(0,original.MaterialSection.InnerFilePositionOfFirstTracks);
				
			var newFile = PrepareFullFile(
				original.BytesBeforeFile,
				innerFileUpToFirstTrack, 
				blackboard.Tracks, 
				original.BytesAfterFile);
			blackboard.FileBytes = newFile;
		}
		
		private static byte[] PrepareFullFile(byte[] fileUpToMaterialSection, byte[] innerFileUpToFirstTrack, List<TemporaryTrack> tracks, byte[] endOfFilePortion)
		{
			List<byte[]> fileSequence = new List<byte[]>();
			fileSequence.Add(fileUpToMaterialSection);
			fileSequence.Add(innerFileUpToFirstTrack);
			foreach (var track in tracks)
			{
				fileSequence.Add(track.HeaderPortion);
				fileSequence.Add(track.HcaPortion);

				var hexAlignment = (track.HeaderPortion.Length + track.HcaPortion.Length) % 16;
				int remainder = (16 - hexAlignment) % 16;
				if (remainder != 0)
					fileSequence.Add(new byte[remainder]);
			}

			fileSequence.Add(endOfFilePortion);

			var fullFile = fileSequence.SelectMany(x => x).ToArray();
			return fullFile;
		}
	}
}