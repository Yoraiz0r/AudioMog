using System;
using System.Collections.Generic;

namespace AudioMog.Application
{
	public class ServiceProvider
	{
		private readonly IApplicationLogger _logger;
		private readonly ApplicationSettings _settings;
		private readonly Dictionary<Type, AService> _cache = new Dictionary<Type, AService>();

		public ServiceProvider(IApplicationLogger logger, ApplicationSettings settings)
		{
			_logger = logger;
			_settings = settings;
		}

		public T GetService<T>(bool forceNewInstance = false) where T : AService, new()
		{
			if (!forceNewInstance && _cache.TryGetValue(typeof(T), out var cachedService))
				return (T)cachedService;

			var tInstance = new T
			{
				Logger = _logger,
				Settings = _settings
			};

			if (!forceNewInstance)
				_cache[typeof(T)] = tInstance;

			return tInstance;
		}
	}

}