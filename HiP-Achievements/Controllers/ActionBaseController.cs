﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;

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
        protected readonly EntityIndex _entityIndex;
        protected readonly EventStoreService _eventStore;
        // ReSharper Restore All


        public ActionBaseController(EventStoreService eventStore, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post([FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResult = await ValidateActionArgs(args);
            if (!validationResult.Success)
                return validationResult.ActionResult;

            var id = _entityIndex.NextId(ResourceTypes.Action);
            await EntityManager.CreateEntityAsync(_eventStore, args, ResourceType, id, User.Identity.GetUserIdentity());            
            return Created($"{Request.Scheme}://{Request.Host}/api/Action/{id}", id);
        }

    }

}
