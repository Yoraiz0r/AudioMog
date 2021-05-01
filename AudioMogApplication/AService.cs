namespace AudioMog.Application
{
	public abstract class AService
	{
		public ApplicationSettings Settings { set; protected get; }
		public IApplicationLogger Logger { set; protected get; }

		public abstract void Run();
	}
}