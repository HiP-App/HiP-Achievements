using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Action = PaderbornUniversity.SILab.Hip.Achievements.Model.Entity.Action;
using ActionResult = PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.ActionResult;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ActionController : Controller
    {
        private readonly EventStoreService _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;

        public ActionController(EventStoreService eventStore, CacheDatabaseManager db, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _db = db;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpGet]
        [ProducesResponseType(typeof(AllItemsResult<ActionResult>), 200)]
        public IActionResult GetAllActions()
        {
            var query = _db.Database.GetCollection<Action>(ResourceType.Action.Name).AsQueryable();
            var userId = User.Identity.GetUserIdentity();
            var result = query.Where(x => x.UserId == userId).ToList()
                              .Select(x => new ActionResult(x))
                              .ToList();
            return Ok(new AllItemsResult<ActionResult>() { Total = result.Count, Items = result });
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] ActionArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            /// TODO : Validation check (e.g. exhibit exist) and User haven`t been there yet
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
