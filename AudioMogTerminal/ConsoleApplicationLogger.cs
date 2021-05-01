using System;
using AudioMog.Application;

namespace AudioMog.Terminal
{
	public class ConsoleApplicationLogger : IApplicationLogger
	{
		public ProgramLogLevel LogLevel = ProgramLogLevel.Everything;
		public void Log(string message)
		{
			if ((int)LogLevel <  (int)ProgramLogLevel.Everything)
				return;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(message);
		}

		public void Warn(string message)
		{
			if ((int)LogLevel <  (int)ProgramLogLevel.ErrorsAndWarnings)
				return;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
		}

		public void Error(string message)
		{
			if ((int)LogLevel <  (int)ProgramLogLevel.ErrorsOnly)
				return;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
		}
	}
}