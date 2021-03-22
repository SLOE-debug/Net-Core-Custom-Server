using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nowin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Server
{
    public class FormOpt : IOptions<FormOptions>
    {
        public FormOptions Value => new FormOptions();
    }
    class Program
    {
        private static INowinServer NowinServer;
        static void Main(string[] args)
        {
            ServerBuilder server = new ServerBuilder();
            var ConfigBuilder = new ConfigurationBuilder();
            ConfigBuilder.AddJsonFile("appsettings.json", true, true);
            var Config = ConfigBuilder.Build();

            var SerCollection = new ServiceCollection();
            SerCollection.AddSingleton<IOptions<FormOptions>, FormOpt>();

            var DefaultHttpContext = new DefaultHttpContextFactory(new DefaultServiceProviderFactory().CreateServiceProvider(SerCollection));

            Func<IDictionary<string, object>, Task> appFunc = async env =>
            {
                var features = new FeatureCollection(new OwinFeatureCollection(env));
                DefaultHttpContext.Create(features);
                var context = DefaultHttpContext.Create(features);
                Console.WriteLine(context.Request.GetEncodedPathAndQuery());
                context.Response.Headers.Add("content-Type", "application/json;charset=UTF-8");
                await context.Response.WriteAsync("Hello World！");
            };
            appFunc = OwinWebSocketAcceptAdapter.AdaptWebSockets(appFunc);

            IPAddress ip = IPAddress.Loopback;
            int port = 3000;
            var url = Config["Host:Url"];
            if (url != null)
            {
                var uri = new Uri(url);
                if (uri.Host.ToLower() != "localhost") IPAddress.TryParse(uri.Host, out ip);
                port = uri.Port;
            }

            NowinServer = server.SetAddress(ip).SetPort(port).SetOwinApp(appFunc).Build();

            Console.WriteLine($"监听在：{ip}：{port}");

            NowinServer.Start();
            Console.ReadLine();

            //var host = new WebHostBuilder()
            //    .ConfigureAppConfiguration(configbuilder =>
            //    {
            //        configbuilder.AddJsonFile("appsettings.json", true, true);
            //    })
            //    .ConfigureServices(Services =>
            //    {
            //        Services.AddSingleton<IServer, UnfinishedServer>();
            //    })
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseStartup<Startup>()
            //    .Build();

            //host.Run();
        }
        ~Program()
        {
            NowinServer.Dispose();
        }
    }
}
