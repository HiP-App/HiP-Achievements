using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private readonly EventStoreClient _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;


        public AchievementsController(/*EventStoreClient eventStore, CacheDatabaseManager db,*/ InMemoryCache cache)
        {
            //_eventStore = eventStore;
            //_db = db;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetAllAchievements()
        {
            return Ok(new List<int> { 1, 2, 3, 4 });
        }

        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [HttpGet("{id}")]
        public IActionResult GetAchievementById(int id)
        {
            var achievement1 = new AchievementResult
            {
                Id = 1,
                Description = "Visit 10 exhibits to get this achievement",
                Title = "Visit 10 exhibits",
                ImageUrl = "http://sample.image",
                NextId = 2,
                Status = AchievementStatus.Published,
                Type = AchievementType.ExhibitVisisted
            };
            var achievement2 = new AchievementResult
            {
                Id = 2,
                Description = "Visit 20 exhibits to get this achievement",
                Title = "Visit 20 exhibits",
                ImageUrl = "http://sample.image",
                NextId = 3,
                Status = AchievementStatus.Published,
                Type = AchievementType.ExhibitVisisted
            };
            var achievement3 = new AchievementResult
            {
                Id = 3,
                Description = "Visit 30 exhibits to get this achievement",
                Title = "Visit 40 exhibits",
                ImageUrl = "http://sample.image",
                NextId = -1,
                Status = AchievementStatus.Published,
                Type = AchievementType.ExhibitVisisted
            };
            var achievement4 = new AchievementResult
            {
                Id = 1,
                Description = "Visit all exhibits on the Karls Route to get this achievement",
                Title = "Finish Karls Route",
                ImageUrl = "http://sample.image",
                NextId = -1,
                Status = AchievementStatus.Unpublished,
                Type = AchievementType.ExhibitVisisted
            };

            var achievements = new[] { achievement1, achievement2, achievement3, achievement4 };

            var result = achievements.FirstOrDefault(a => a.Id == id);
            if (result == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            return Ok(result);
        }

    }
}
