using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System.Diagnostics;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.Channels;
using CoreWCF;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            //IWebHost host = CreateWebHostBuilder(args).Build();
            //host.Run();

            var builder = WebApplication.CreateBuilder(args);

            // Add WSDL support
            builder.Services.AddServiceModelServices().AddServiceModelMetadata();
            // Use the scheme/host/port used to fetch WSDL as that service endpoint address in generated WSDL 
            builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
            var app = builder.Build();

            app.UseServiceModel(builder =>
            {
                builder.AddService<MyService>((serviceOptions) =>
                {
                    // Set the default host name:port in generated WSDL and the base path for the address 
                    serviceOptions.BaseAddresses.Add(new Uri($"http://localhost/MyService"));
                    serviceOptions.BaseAddresses.Add(new Uri($"https://localhost/MyService"));
                })
                .AddServiceEndpoint<MyService, IMyService>(new BasicHttpBinding(), "/basicHttp")
                .AddServiceEndpoint<MyService, IMyService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/basicHttp");
            });

            // Enable WSDL for http & https
            var serviceMetadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;

            app.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
           .UseKestrel(options => {
               options.ListenAnyIP(8088);
               options.ListenAnyIP(8443, listenOptions =>
               {
                   listenOptions.UseHttps();
                   if (Debugger.IsAttached)
                   {
                       listenOptions.UseConnectionLogging();
                   }
               });
               options.AllowSynchronousIO = true;
           })
           .UseStartup<Startup>();
    }
}