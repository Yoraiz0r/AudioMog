using System.IO;
using VGAudio.Containers.Hca;

namespace AudioMog.Application.AudioFileRebuilder
{
	public class ExposedHcaReader : HcaReader
	{
		public HcaStructure GetStructure(byte[] hcaBytes)
		{
			var memoryStream = new MemoryStream(hcaBytes);
			return ReadFile(memoryStream);
		}
	}
}