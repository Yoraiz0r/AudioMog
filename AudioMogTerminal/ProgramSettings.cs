using System;
using AudioMog.Application;

namespace AudioMog.Terminal
{
	[Serializable]
	public class ProgramSettings
	{
		public TerminalSettings TerminalSettings;
		public ApplicationSettings ApplicationSettings;
	}
}