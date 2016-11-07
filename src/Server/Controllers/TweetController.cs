using System;
using System.Collections.Generic;
using Core.Cache.Interfaces;
using Core.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class TweetController : Controller
    {
        private readonly ITweetService _tweetService;
        private readonly ICacheService _cache;

        public TweetController(ITweetService tweetService, ICacheService cache)
        {
            _tweetService = tweetService;
            _cache = cache;
        }


        [HttpGet("{key}")]
        public Result<IEnumerable<Tweet>> Get(string key)
        {
            string cacheKey = $"{nameof(TweetController)}-{nameof(Get)}-{key}";
            return _cache.GetOrStore(cacheKey, () => _tweetService.GetTweetByKey(key), TimeSpan.FromDays(1));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tweetService.Dispose();
            }
        }
    }
}