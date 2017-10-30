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
using System.Linq;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    /// <summary>
    /// Base class for creating controllers for a specific action type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>
    [Authorize]
    [Route("api/Actions/[controller]")]

    public abstract class ActionBaseController<TArgs> : Controller where TArgs : ActionArgs
    {

        private readonly EntityIndex _entityIndex;
        private readonly EventStoreClient _eventStore;

        public ActionBaseController(EventStoreClient eventStore, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post([FromBody] ActionsArgs args) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var argList = args.ToListActionArgs();
            var validationResultList = new List<(int,ArgsValidationResult)>();

            foreach (var arg in argList)
            {
                validationResultList.Add((arg.EntityId, await ValidateActionArgs(arg)));
                if (!validationResultList.Last().Item2.Success)
                {
                    continue;
                }

                var ev = new ActionCreated
                {
                    Id = _entityIndex.NextId(ResourceType.Action),
                    UserId = User.Identity.GetUserIdentity(),
                    Properties = arg,
                    Timestamp = DateTimeOffset.Now
                };

                await _eventStore.AppendEventAsync(ev);
            }
            if (validationResultList.Any(x => x.Item2.Success))
            {
                return StatusCode(201, String.Join(',', validationResultList.FindAll(x => x.Item2.Success)
                                                                            .Select(x => x.Item1)
                                                                            .ToArray()));
            }
            else
            {
                return StatusCode(400, validationResultList.Select(x => x.Item1).ToArray());
            }
        }

        protected abstract Task<ArgsValidationResult> ValidateActionArgs(ActionArgs args);

    }

}
