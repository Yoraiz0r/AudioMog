using System.IO;
using System.Text;
using AudioMog.Application.Utilities;
using StbVorbisSharp;

namespace AudioMog.Application.Codecs
{
	public class OggVorbisToWavSampleWriter : IWavSampleWriter
	{
		private Vorbis _vorbis;

		public OggVorbisToWavSampleWriter(Vorbis vorbis)
		{
			_vorbis = vorbis;
		}

		public void WriteSamples(BinaryWriter writer)
		{
			var vorbis = _vorbis;
			
			writer.Write(Encoding.ASCII.GetBytes("data"));
			writer.Flush();
			var dataPos = writer.BaseStream.Position;
			writer.Write(0);//size bytes, for later
			
			vorbis.SubmitBuffer();
			while (vorbis.Decoded != 0)
			{
				var audioShort = vorbis.SongBuffer;
				for (var i = 0; i < vorbis.Decoded * vorbis.Channels; ++i)
					writer.Write(audioShort[i]);

				vorbis.SubmitBuffer();
			}
			writer.Flush();

			var length = writer.BaseStream.Position;
			
			writer.Seek((int)dataPos, SeekOrigin.Begin);
			writer.Write((int)(length - dataPos - 4L));
			
			writer.Seek((int)length, SeekOrigin.Begin);
		}
	}
}