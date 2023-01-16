using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable CoreWCF Services, enable metadata
            // Use the Url used to fetch WSDL as that service endpoint address in generated WSDL 
            services.AddServiceModelServices()
                    .AddServiceModelMetadata()
                    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            //services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            //        .AddNegotiate();

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                var binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.None;
                //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

                builder.AddService<MyService>(serviceOptions =>
                {
                    serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
                    // Set the default host name:port in generated WSDL and the base path for the address 
                    serviceOptions.BaseAddresses.Add(new Uri($"http://localhost/MyService"));
                    serviceOptions.BaseAddresses.Add(new Uri($"https://localhost/MyService"));
                })
                // Add WSHttpBinding endpoints
                //.AddServiceEndpoint<MyService, IMyService>(binding, "/ws")
                //.AddServiceEndpoint<MyService, IMyService>(new BasicHttpBinding(), "/bh");
                .AddServiceEndpoint<MyService, IMyService>(new BasicHttpBinding(), "/basicHttp")
                .AddServiceEndpoint<MyService, IMyService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/basicHttp");
            });

            // Configure WSDL to be available over http & https
            var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;
        }
    }
}
