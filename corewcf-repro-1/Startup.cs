using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

namespace corewcf_repro_1
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // WSDL support
            services.AddServiceModelServices().AddServiceModelMetadata();
            services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            // Inject CoreWCF services into the application to support
            // dependency injection into WCF services
            services.AddSingleton<EchoService.EchoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable CoreWCF SOAP metadata generation on https GET
            var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpGetEnabled = true;
            serviceMetadataBehavior.HttpsGetEnabled = true;

            // CoreWCF SOAP services
            app.UseServiceModel(wcfBuilder =>
            {
                wcfBuilder.AddService<EchoService.EchoService>((serviceOptions) =>
                    {
                        // serviceOptions.BaseAddresses.Add(new Uri("https://localhost:44340/VirtualFolder/"));
                    })
                    .AddServiceEndpoint<EchoService.EchoService, EchoService.IEchoService>(
                        new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/ws/EchoService.svc");
            });
        }
    }
}