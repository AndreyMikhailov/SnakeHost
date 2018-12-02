using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnakeHost.Logic;

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
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

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
            
            // Configure game API
            app.UseMvc();
        }

        private readonly Settings _settings;
    }
}