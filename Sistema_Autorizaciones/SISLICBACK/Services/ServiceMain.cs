namespace SISLICBACK.Services {
    public static class ServiceMain {
        public static void RegisterServices(IServiceCollection services) { 
            services.AddHttpClient<SolicitanteService>();
            services.AddScoped<SolicitanteService>();

            services.AddHttpClient<EstablecimientoService>();
            services.AddScoped<EstablecimientoService>();

            services.AddHttpClient<SolicitudService>();
            services.AddScoped<SolicitudService>();

            services.AddHttpClient<SesionesService>();
            services.AddScoped<SesionesService>();

            services.AddHttpClient<ArchivosService>();
            services.AddScoped<ArchivosService>();

            services.AddHttpClient<MarkMapsService>();
            services.AddScoped<MarkMapsService>();

            services.AddHttpClient<AnexosService>();
            services.AddScoped<AnexosService>();

            services.AddHttpClient<GiroService>();
            services.AddScoped<GiroService>();

            services.AddHttpClient<GraficosServices>();
            services.AddScoped<GraficosServices>();

            services.AddHttpClient<ConsultaService>();
            services.AddScoped<ConsultaService>();

            services.AddHttpClient<CrudEPEService>();
            services.AddScoped<CrudEPEService>();

        }
    }
}
