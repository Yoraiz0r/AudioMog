namespace AudioMog.Application
{
	public interface IApplicationLogger
	{
		void Log(string message);
		void Warn(string message);
		void Error(string message);
	}
}