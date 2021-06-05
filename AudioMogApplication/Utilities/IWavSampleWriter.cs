using System.IO;

namespace AudioMog.Application.Utilities
{
	public interface IWavSampleWriter
	{
		void WriteSamples(BinaryWriter writer);
	}
}