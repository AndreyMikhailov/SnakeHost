using System;
using System.IO;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnakeHost.Logic;
using Swashbuckle.AspNetCore.Swagger;

namespace SnakeHost
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();

            _settings = Configuration.Get<Settings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure DDOS protection
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            
            // Generate docs
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Snake API", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(Environment.CurrentDirectory, "SnakeHost.xml"));
                c.DescribeAllEnumsAsStrings();
            });

            // Configure game API
            services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
            services.AddSingleton(_settings);
            services.AddSingleton<Authenticator>();
            services.AddSingleton<Game>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Configure DDOS protection
            app.UseIpRateLimiting();
            app.UseClientRateLimiting();
            
            // Generate docs
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Snake API");
            });

            // Configure game API
            app.UseMvc();
        }

        private readonly Settings _settings;
    }
}