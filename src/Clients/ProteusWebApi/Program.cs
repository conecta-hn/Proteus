/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus
{
    public class Program
    {
        internal static HashSet<ProteusAspModule> _modules;
        public static async Task Main(string[] args)
        {
            CreateWebHostBuilder(args)
#if DEBUG
                .UseUrls("https://0.0.0.0:44363", "http://0.0.0.0:44364")
#endif
                .Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}