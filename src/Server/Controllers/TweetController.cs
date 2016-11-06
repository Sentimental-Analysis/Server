using System.Collections.Generic;
using Core.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class TweetController : Controller
    {
        private readonly ITweetService _tweetService;

        public TweetController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }


        [HttpGet("{key}")]
        public Result<IEnumerable<Tweet>> Get(string key)
        {
            using (_tweetService)
            {
                return _tweetService.GetTweetByKey(key);
            }

        }
    }
}