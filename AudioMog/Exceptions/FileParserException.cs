using System;

namespace AudioMog.Core.Exceptions
{
	public class FileParserException : Exception
	{
		public FileParserException()
		{
		}
		public FileParserException(string message) : base(message)
		{
		}
	}
}