﻿using System;
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
    /// Base class for creating controllers for a specific action type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>
    [Authorize]
    [Route("api/Actions/[controller]")]

    public class ActionBaseController<TArgs> : Controller where TArgs : ActionArgs
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
        public async Task<IActionResult> Post([FromBody] TArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // TODO : Validation check (e.g. exhibit exist) and User haven`t been there yet
            //switch (args.Type)
            //{
            //    case Model.Entity.ActionType.ExhibitVisited:
            //        if (!_entityIndex.Exists(ResourceType.))
            //            ModelState.AddModelError("Exhibit", String.Format("Exhibit with Id: {0} doesn`t exist", args.EntityId));
            //        break;
            //}

            var ev = new ActionCreated()
            {
                Id = _entityIndex.NextId(ResourceType.Action),
                UserId = User.Identity.GetUserIdentity(),
                Properties = args,
                Timestamp = DateTimeOffset.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return Created($"{Request.Scheme}://{Request.Host}/api/Action/{ev.Id}", ev.Id);
        }
    }
}
