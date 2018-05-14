using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using System;
using System.Threading.Tasks;
using ActionArgs = PaderbornUniversity.SILab.Hip.UserStore.ActionArgs;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    /// <summary>
    /// Base class for creating controllers for a specific action type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>

    [Authorize]
    [Route("api/Actions/[controller]")]
    public abstract class ActionBaseController<TArgs> : BaseController<TArgs> where TArgs : ActionArgs, new()
    {
        // ReSharper disable All
        protected readonly EventStoreService _eventStore;
        protected readonly EntityIndex _entityIndex;
        // ReSharper Restore All


        public ActionBaseController(EventStoreService eventStore, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [Obsolete("Use UserStore instead")]
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> Post([FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (User.Identity.GetUserIdentity() == null)
                return Forbid();

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;

            var id = _entityIndex.NextId(ResourceTypes.Action);
            await EntityManager.CreateEntityAsync(_eventStore, args, ResourceType, id, User.Identity.GetUserIdentity());
            return Created($"{Request.Scheme}://{Request.Host}/api/Action/{id}", id);
        }
    }
}
