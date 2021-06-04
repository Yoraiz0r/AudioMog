using AudioMog.Application.Codecs;

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
			var codec = AvailableCodecs.GetCodec(track.CurrentCodec);
			if (codec == null)
				return;

			codec.FixHeader(track);
		}
	}
}