using System.Linq;
using AudioMog.Core;
using AudioMog.Core.Audio;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class PrintTrackHeadersStep : ARebuilderStep
	{
		public override void Run(Blackboard blackboard)
		{
			foreach (var track in blackboard.Tracks)
			{
				var originalEntry = track.OriginalEntry;
				var extraDataStart = originalEntry.ExtraDataOffset;
				var extraDataSize = originalEntry.ExtraDataSize;
				if (track.CurrentCodec == MaterialCodecType.HCA)
					extraDataSize = 16;
				var subarray = blackboard.FileBytes.SubArray(extraDataStart, extraDataSize)
					.Select(x => 
						x.ToString("X2")
							.Replace("0", "_")
					)
					.ToArray();
				var joinedString = string.Join(",", subarray);
				if (subarray.Length >= 100)
					joinedString = "over 100!";
				blackboard.Logger.Log($"Entry {originalEntry.EntryIndex:D3}: {joinedString}");
			}
		}
	}
}