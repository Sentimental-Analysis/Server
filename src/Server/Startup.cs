using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bayes.Classifiers.Implementations;
using Bayes.Classifiers.Interfaces;
using Bayes.Data;
using Bayes.Learner.Implementations;
using Bayes.Learner.Interfaces;
using Core.Cache.Implementations;
using Core.Cache.Interfaces;
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
using Server.Utils;

namespace Server
{
    public class Startup
    {
        private readonly string _afinnPath;

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

            _afinnPath = $"{env.WebRootPath}{Path.DirectorySeparatorChar}data{Path.DirectorySeparatorChar}afinn.json";
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<ICacheService, InMemoryCacheService>();

            services.AddScoped<IUnitOfWork>(provider =>
                {
                    var cluster =
                        Cassandra.Cluster.Builder().AddContactPoint("127.0.0.1").WithDefaultKeyspace("sentiment").Build();

                    return new DefaultUnitOfWork(cluster, new TwitterApiCredentials
                    {
                        AccessToken = Configuration["TwitterCredentials:ACCESS_TOKEN"],
                        AccessTokenSecret = Configuration["TwitterCredentials:ACCSESS_TOKEN_SECRET"],
                        ConsumerKey = Configuration["TwitterCredentials:CONSUMER_KEY"],
                        ConsumerSecret = Configuration["TwitterCredentials:CONSUMER_SECRET"]
                    });
                }
            );

            services.AddScoped<ITweetLearner, TweetLearner>();

            services.AddScoped<ILearningService>(provider =>
            {
                var initState =
                    new Lazy<IEnumerable<Sentence>>(
                        () =>
                            FileUtils.GetAfinnJsonFile(_afinnPath)
                                .Select(
                                    x =>
                                        new Sentence(x.Key, x.Value >= 0 ? WordCategory.Positive : WordCategory.Negative))
                                .ToList());

                var cacheService = provider.GetRequiredService<ICacheService>();
                var learner = provider.GetRequiredService<ITweetLearner>();
                return new BayesLearningService(cacheService, learner, initState);
            });

            services.AddScoped<ITweetClassifier>(provider =>
            {
                var learningService = provider.GetRequiredService<ILearningService>();
                return new TweetClassifier(learningService.Get());
            });

            services.AddScoped<ISentimentalAnalysisService>(provider =>
            {
                var learningService = provider.GetRequiredService<ILearningService>();
                var classifier = provider.GetRequiredService<ITweetClassifier>();
                return new BayesAnalysisService(learningService, classifier);
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
            services.AddCors(options =>
            {
                options.AddPolicy("AnyOrigin", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("AnyOrigin").UseMvc().UseSwagger().UseSwaggerUi();
        }
    }
}