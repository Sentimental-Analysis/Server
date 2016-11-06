using System.Collections.Immutable;
using Core.Cache.Implementations;
using Core.Cache.Interfaces;
using Core.Database.Implementations;
using Core.Database.Interfaces;
using Core.Models;
using Core.Services.Implementations;
using Core.Services.Interfaces;
using Core.UnitOfWork.Implementations;
using Core.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<PostgresDbContext>(
                    options => options.UseNpgsql(Configuration["Data:DbContext:LocalConnectionString"]));
            services.AddDbContext<PostgresDbContext>();
            services.AddScoped<IDbContext, PostgresDbContext>();
            services.AddScoped<ISentimentalAnalysisService>(
                provider => new SimpleAnalysisService(ImmutableDictionary<string, int>.Empty));
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<IDbContext>();
                var cache = provider.GetRequiredService<IMemoryCache>();
                var sentimentalAnalysisService = provider.GetRequiredService<ISentimentalAnalysisService>();
                return new DefaultUnitOfWork(dbContext, cache, new TwitterApiCredentials(), sentimentalAnalysisService);
            });
            services.AddScoped<ITweetService, TweetService>();

            // Add framework services.
            services.AddMvc();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc().UseSwagger().UseSwaggerUi();
            ;
        }
    }
}