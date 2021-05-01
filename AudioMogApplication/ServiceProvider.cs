namespace AudioMog.Application
{
	public class ServiceProvider
	{
		private readonly IApplicationLogger _logger;
		private readonly ApplicationSettings _settings;

		public ServiceProvider(IApplicationLogger logger, ApplicationSettings settings)
		{
			_logger = logger;
			_settings = settings;
		}

		public T GetService<T>() where T : AService, new()
		{
			var tInstance = new T();
			tInstance.Logger = _logger;
			tInstance.Settings = _settings;
			return tInstance;
		}
	}
}