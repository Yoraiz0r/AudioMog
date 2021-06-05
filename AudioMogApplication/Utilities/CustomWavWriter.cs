using System.IO;
using System.Text;

namespace AudioMog.Application.Utilities
{
	public class CustomWavWriter
	{
		public void WriteWav(CustomWavWriterRequest request)
		{
			using (var outFile = File.Create(request.WavPath))
			using (var writer = new BinaryWriter(outFile))
			{
				WriteStartChunk(writer);
				WriteFmtChunk(writer, request.Channels, request.SampleRate);
				WriteDataChunk(writer, request.WavSampleWriter);
				WriteSamplerChunk(writer, request.LoopPoints);
				
				writer.Flush();
				
				WriteSizeToStart(writer);
			}
		}

		private static void WriteSizeToStart(BinaryWriter writer)
		{
			var length = writer.BaseStream.Position;
			writer.Seek(4, SeekOrigin.Begin);
			writer.Write((int) (length - 8));
		}

		private static void WriteStartChunk(BinaryWriter writer)
		{
			writer.Write(Encoding.ASCII.GetBytes("RIFF"));
			writer.Write(0);
			writer.Write(Encoding.ASCII.GetBytes("WAVE"));
		}

		private static void WriteFmtChunk(BinaryWriter writer, int channels, int sampleRate)
		{
			writer.Write(Encoding.ASCII.GetBytes("fmt "));
			writer.Write(18); //subchunk size
			writer.Write((short) 1); // PCM format
			writer.Write((short) channels);
			writer.Write(sampleRate);
			writer.Write(sampleRate * channels * 2); // avg bytes per second
			writer.Write((short) (2 * channels)); // block align
			writer.Write((short) 16); // bits per sample
			writer.Write((short) 0); // extra size
		}

		private static void WriteDataChunk(BinaryWriter writer, IWavSampleWriter sampleWriter)
		{
			sampleWriter?.WriteSamples(writer);
		}

		private static void WriteSamplerChunk(BinaryWriter writer, WavWriterLoopPoint[] loopPoints)
		{
			const int samplerDataCount = 0;
			
			if (loopPoints == null)
				loopPoints = new WavWriterLoopPoint[0];
			
			writer.Write(Encoding.ASCII.GetBytes("smpl"));
			writer.Write(36 + loopPoints.Length * 24 + samplerDataCount); //0x04 size
			writer.Write(0); //0x08 manufacturer
			writer.Write(0); //0x0c product
			writer.Write(0); //0x10 sample period
			writer.Write(0); //0x14 midi unity note
			writer.Write(0); //0x18 midi pitch fraction
			writer.Write(0); //0x1c smpte format
			writer.Write(0); //0x20 smpte offset
			writer.Write(loopPoints.Length); //number of sample loops
			writer.Write(samplerDataCount); //sampler data - byte count that will follow this chunk (including the entire sample loop list)
			foreach (var loopPoint in loopPoints)
				WriteSamplerChunk_WriteLoop(writer, loopPoint);
		}

		private static void WriteSamplerChunk_WriteLoop(BinaryWriter writer, WavWriterLoopPoint wavWriterLoopPoint)
		{
			writer.Write(0); //id
			writer.Write(0); //type, 0 = forward(normal)
			writer.Write(wavWriterLoopPoint.StartSample);
			writer.Write(wavWriterLoopPoint.EndSample);
			writer.Write(0); //fraction
			writer.Write(0); //loop count, 0 = infinite
		}
	}
}