using System;
using AudioMog.Application.AudioExtractor;
using AudioMog.Core;

namespace AudioMog.Application
{
	[Serializable]
	public class ApplicationSettings
	{
		public FileParserSettings Parser;
		public AudioExtractorSettings AudioExtractor;
	}
}