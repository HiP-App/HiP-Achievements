using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    /// <summary>
    /// Base class for creating controllers for a specific achievement type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>
    [Authorize]
    [Route("api/Achievements/[controller]")]
    public abstract class AchievementBaseController<TArgs> : BaseController<TArgs> where TArgs : AchievementArgs
    {
        private readonly EntityIndex _entityIndex;
        private readonly EventStoreClient _eventStore;

        public AchievementBaseController(EventStoreClient eventStore, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int),201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateAchievement([FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserPermissions.IsAllowedToCreate(User.Identity, args.Status))
                return Forbid();

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;

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

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_entityIndex.Exists(ResourceType.Achievement, id))
                return NotFound();

            if (!UserPermissions.IsAllowedToEdit(User.Identity, args.Status, _entityIndex.Owner(ResourceType.Achievement, id)))
                return Forbid();

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;

            var ev = new AchievementUpdated
            {
                Id = id,
                Properties = args,
                UserId = User.Identity.GetUserIdentity(),
                Timestamp = DateTime.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return NoContent();
        }
        
    }
}
