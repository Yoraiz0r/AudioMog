using System.IO;

namespace AudioMog.Application.Codecs
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