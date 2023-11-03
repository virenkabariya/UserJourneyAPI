namespace UserJourney.API.Code
{
    public class Context
    {
        private static IConfiguration _config;
        private static object _configLock = new object();

        private static IServiceCollection _services;
        private static object _servicesLock = new object();

        public static IServiceCollection ServiceCollection
        {
            get
            {
                lock (Context._servicesLock)
                {
                    return Context._services;
                }
            }
        }

        public static IConfiguration Configuration
        {
            get
            {
                lock (Context._configLock)
                {
                    return Context._config;
                }
            }
        }

        public static void SetConfiguration(IConfiguration config)
        {
            lock (Context._configLock)
            {
                Context._config = config;
            }
        }

        public static void SetServiceCollection(IServiceCollection services)
        {
            lock (Context._servicesLock)
            {
                Context._services = services;
            }
        }
    }
}
