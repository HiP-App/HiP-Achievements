using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    /// <summary>
    /// Base class for creating controllers for a specific achievement type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>
    [Authorize]
    [Route("api/Achievements/[controller]")]
    public abstract class AchievementBaseController<TArgs> : BaseController<TArgs> where TArgs : AchievementArgs, new()
    {
        private static object _lockObject = new object();
        private readonly EntityIndex _entityIndex;
        private readonly EventStoreService _eventStore;

        public AchievementBaseController(EventStoreService eventStore, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateAchievement([FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserPermissions.IsAllowedToCreate(User.Identity, args.Status) || User.Identity.GetUserIdentity() == null)
                return Forbid();

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;
            var id = _entityIndex.NextId(ResourceTypes.Achievement);
            await EntityManager.CreateEntityAsync(_eventStore, args, ResourceType, id, User.Identity.GetUserIdentity());

            //await _eventStore.AppendEventAsync(ev);
            return Created($"{Request.Scheme}://{Request.Host}/api/Achievements/{id}", id);
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

            if (!_entityIndex.Exists(ResourceTypes.Achievement, id))
                return NotFound();

            if (!UserPermissions.IsAllowedToEdit(User.Identity, args.Status, _entityIndex.Owner(ResourceTypes.Achievement, id)) || User.Identity.GetUserIdentity() == null)
                return Forbid();

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;

            var currentArgs = await EventStreamExtensions.GetCurrentEntityAsync<TArgs>(_eventStore.EventStream, ResourceType, id);
            await EntityManager.UpdateEntityAsync(_eventStore, currentArgs, args, ResourceType, id, User.Identity.GetUserIdentity());
            return NoContent();
        }

    }
}
