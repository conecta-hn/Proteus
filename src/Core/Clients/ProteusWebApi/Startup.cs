/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Plugins;
using System.Collections.Generic;
using TheXDS.MCART.PluginSupport;

namespace TheXDS.Proteus
{
    public class Startup
    {
        public static ProteusSettings Settings { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options=> {
                options.AddPolicy("corsPolicy",b=> {
                    b.SetIsOriginAllowedToAllowWildcardSubdomains();
                    b.WithOrigins("http://10.172.3.16").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                    b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
            });
            var mvc = services.AddMvc();
            mvc.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            Settings = Configuration.GetSection("ProteusSettings").Get<ProteusSettings>();
            ProteusLib.MessageTarget = new LogcatMessageTarget();
            ProteusLib.Init(Settings).GetAwaiter().GetResult();
            ProteusLib.Login(Settings.ServiceApiToken).GetAwaiter().GetResult();

            var pl = new PluginLoader();
            Program._modules = new HashSet<ProteusAspModule>(pl.LoadEverything<ProteusAspModule>(Settings.WebPluginsDir));

            foreach (var j in Program._modules)
            {
                mvc.AddApplicationPart(j.Assembly).AddControllersAsServices();
            }
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Proteus Web API",
                    Version = "v1"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("corsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proteus Web API v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
