using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nowin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Server
{
    public static class ServerBuilderExtend
    {
        public static void SetUrl(this ServerBuilder server, string Url)
        {
            IPAddress ip = IPAddress.Loopback;
            int port = 3000;
            var uri = new Uri(Url);
            if (uri.Host.ToLower() != "localhost") IPAddress.TryParse(uri.Host, out ip);
            port = uri.Port;

            server.SetAddress(ip).SetPort(port);
        }
    }
    public class CustomServer
    {
        private ServerBuilder ServerBuilder { get; set; }

        private ContainerBuilder Container { get; set; }

        private IConfigurationBuilder ConfigurationBuilder { get; set; }

        private IConfiguration Configuration { get; set; }

        private INowinServer Server;

        private DefaultHttpContextFactory DefaultHttpContextFactory { get; set; }

        private ServiceCollection SerCollection { get; set; }

        public CustomServer()
        {
            ServerBuilder = new ServerBuilder();
            Container = new ContainerBuilder();
            ConfigurationBuilder = new ConfigurationBuilder();
            SerCollection = new ServiceCollection();
            SerCollection.AddSingleton<IOptions<FormOptions>>(services =>
            {
                return new OptionsManager<FormOptions>(new OptionsFactory<FormOptions>(new List<IConfigureOptions<FormOptions>> { new ConfigureOptions<FormOptions>(formoption => { }) }, new List<IPostConfigureOptions<FormOptions>>() { }));
            });
        }

        public CustomServer InjectDI(Action<ContainerBuilder> Inject)
        {
            Inject(Container);
            return this;
        }

        public CustomServer Config(Action<IConfigurationBuilder> Config)
        {
            Config(ConfigurationBuilder);
            Configuration = ConfigurationBuilder.Build();
            return this;
        }

        public CustomServer ServerConfig(Action<ServerBuilder> ServerConfig)
        {
            ServerConfig(ServerBuilder);
            return this;
        }

        public CustomServer ServerConfig(Action<ServerBuilder, IConfiguration> ServerConfig)
        {
            ServerConfig(ServerBuilder, Configuration);
            return this;
        }

        //public CustomServer SetFormOption(Action<FormOptions> setForm)
        //{
        //    SerCollection.AddSingleton<IOptions<FormOptions>>(services =>
        //    {
        //        return new OptionsManager<FormOptions>(new OptionsFactory<FormOptions>(new List<IConfigureOptions<FormOptions>> { new ConfigureOptions<FormOptions>(setForm) }, new List<IPostConfigureOptions<FormOptions>>() { }));
        //    });
        //    return this;
        //}

        public Task OwinApp(IDictionary<string, object> env)
        {
            var features = new FeatureCollection(new OwinFeatureCollection(env));
            var context = DefaultHttpContextFactory.Create(features);
            context.Response.Headers.Add("content-Type", "application/json;charset=UTF-8");
            context.Response.WriteAsync("阿巴阿巴");
            return Task.CompletedTask;
        }

        public void Start()
        {
            DefaultHttpContextFactory = new DefaultHttpContextFactory(new DefaultServiceProviderFactory().CreateServiceProvider(SerCollection));

            Server = ServerBuilder.SetOwinApp(OwinApp).Build();
            Server.Start();
            Console.TreatControlCAsInput = false;
            while (true)
            {
                Console.ReadKey(true);
            }
        }

        ~CustomServer()
        {
            Server?.Dispose();
        }
    }
}
