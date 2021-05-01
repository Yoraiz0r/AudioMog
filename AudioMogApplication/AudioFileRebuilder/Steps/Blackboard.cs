using System.Collections.Generic;
using AudioMog.Core.Audio;

namespace AudioMog.Application.AudioFileRebuilder.Steps
{
	public class Blackboard
	{
		public AudioRebuilderProjectSettings Settings;
		public AAudioBinaryFile File;
		public List<TemporaryTrack> Tracks;
		public byte[] FileBytes;
		public IApplicationLogger Logger;
	}
}