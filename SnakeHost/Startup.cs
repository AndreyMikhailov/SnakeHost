using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnakeHost.Logic;

namespace SnakeHost
{
    public class Startup
    {
        public Startup()
        {
            _settings = new ConfigurationBuilder()     
                .AddJsonFile("settings.json")
                .Build()
                .Get<Settings>(); 
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
            services.AddSingleton(_settings);
            services.AddSingleton<Authenticator>();
            services.AddSingleton<Game>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }

        private readonly Settings _settings;
    }
}