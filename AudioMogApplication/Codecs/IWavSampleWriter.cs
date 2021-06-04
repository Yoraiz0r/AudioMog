using System.IO;

namespace AudioMog.Application.Codecs
{
	public interface IWavSampleWriter
	{
		void WriteSamples(BinaryWriter writer);
	}
}