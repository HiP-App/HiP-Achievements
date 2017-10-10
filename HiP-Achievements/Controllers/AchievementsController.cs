using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private readonly EventStoreClient _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;


        public AchievementsController(EventStoreClient eventStore, CacheDatabaseManager db, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _db = db;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpGet("ids")]
        [ProducesResponseType(200)]
        public IActionResult GetAllAchievements()
        {
            var achievements = _entityIndex.AllIds(ResourceType.Achievement);
            return Ok(achievements);
        }

        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [HttpGet("{id}")]
        public IActionResult GetAchievementById(int id)
        {
            var result = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable().FirstOrDefault(a => a.Id == id);
            if (result == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            return Ok(new AchievementResult(result));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostAsync([FromBody] AchievementArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ev = new AchievementCreated
            {
                Id = _entityIndex.NextId(ResourceType.Achievement),
                UserId = User.Identity.GetUserIdentity(),
                Properties = args,
                Timestamp = DateTimeOffset.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return Created($"{Request.Scheme}://{Request.Host}/api/Achievements/{ev.Id}", ev.Id);
        }

    }
}
