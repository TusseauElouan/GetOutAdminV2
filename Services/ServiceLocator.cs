using Microsoft.Extensions.DependencyInjection;


namespace GetOutAdminV2.Services
{
    public static class ServiceLocator
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
