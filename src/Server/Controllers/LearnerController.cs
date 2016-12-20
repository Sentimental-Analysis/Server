using Bayes.Data;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class LearnerController : Controller
    {
        private readonly ILearningService _learningService;

        public LearnerController(ILearningService learningService)
        {
            _learningService = learningService;
        }

        [HttpGet]
        public LearnerState Get()
        {
            return _learningService.Get();
        }
    }
}