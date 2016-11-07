using System.Collections.Immutable;
using Core.Cache.Implementations;
using Core.Cache.Interfaces;
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
using Server.Database.Implementations;

namespace Server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);              

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<TweetDbContext>(
                    options => options.UseNpgsql(Configuration["Data:DbContext:LocalConnectionString"]));

            services.AddScoped<IDbContext, TweetDbContext>();
            services.AddTransient<ICacheService, InMemoryCacheService>();
            services.AddScoped<ISentimentalAnalysisService>(
                provider => new SimpleAnalysisService(ImmutableDictionary<string, int>.Empty));

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<IDbContext>();
                return new DefaultUnitOfWork(dbContext, new TwitterApiCredentials()
                {
                    AccessToken = Configuration["TwitterCredentials:ACCESS_TOKEN"],
                    AccessTokenSecret = Configuration["TwitterCredentials:ACCSESS_TOKEN_SECRET"],
                    ConsumerKey = Configuration["TwitterCredentials:CONSUMER_KEY"],
                    ConsumerSecret = Configuration["TwitterCredentials:CONSUMER_SECRET"]
                });
            });
            services.AddScoped<ITweetService>(provider =>
            {
                var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
                var sentimentalAnalysisService = provider.GetRequiredService<ISentimentalAnalysisService>();
                return new TweetService(unitOfWork, sentimentalAnalysisService);
            });

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

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<TweetDbContext>().Database.Migrate();
            }
        }
    }
}