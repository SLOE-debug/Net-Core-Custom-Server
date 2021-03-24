using Autofac;
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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new CustomServer().Config(config =>
            {
                config.AddJsonFile("appsettings.json", true, true);
            })
            .ServerConfig((server, config) =>
            {
                server.SetUrl(config["Host:Url"]);
            })
            .UsePipe(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Type", "application/json;charset=utf-8");
                Console.WriteLine(context.Request.GetEncodedPathAndQuery());
                await context.Response.WriteAsync("巴拉巴拉");
                await next.Action(context, next.Next);
            })
            .UsePipe(async (context, next) =>
            {
                await context.Response.WriteAsync("阿巴阿巴");
                await next.Action(context, next.Next);
            })
            .Start();
        }
    }
}
