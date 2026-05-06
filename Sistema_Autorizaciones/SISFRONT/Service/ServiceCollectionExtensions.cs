using Microsoft.Extensions.DependencyInjection;
using SISFRONT.Service.Interface;

namespace SISFRONT.Service {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddAppServices(this IServiceCollection services) {
            // servicios
            services.AddScoped<IGirosService, GiroService>();
            services.AddScoped<IPublicidadService, PublicidadService>();
            services.AddScoped<SessionService>();

            //interops o helpers
            services.AddScoped<GoogleMapsInterop>();

            return services;
        }
    }
}
