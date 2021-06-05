namespace AudioMog.Application.Utilities
{
	public class CustomWavWriterRequest
	{
		public string WavPath;
		public int Channels;
		public int SampleRate;
		public IWavSampleWriter WavSampleWriter;
		public WavWriterLoopPoint[] LoopPoints;
	}
	public struct WavWriterLoopPoint
	{
		public uint StartSample;
		public uint EndSample;
	}
}