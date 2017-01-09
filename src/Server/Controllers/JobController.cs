using System;
using System.Diagnostics;
using Core.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly ITweetBotService _botService;

        public JobController(ITweetBotService botService)
        {
            _botService = botService;
        }

        [HttpGet("daily")]
        public int Daily()
        {
            RecurringJob.AddOrUpdate(() => _botService.StoreNew(), Cron.Daily);
            return 1;
        }

        [HttpGet("once")]
        public int Once()
        {
            BackgroundJob.Enqueue(() => _botService.StoreNew());
            return 1;
        }

        [HttpGet("{id}")]
        public int Get(int id)
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget Job Executed"));
            return id;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _botService.Dispose();
            }
        }
    }
}