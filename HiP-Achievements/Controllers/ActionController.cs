using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using Action = PaderbornUniversity.SILab.Hip.Achievements.Model.Entity.Action;
using ActionResult = PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.ActionResult;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ActionController : Controller
    {
        private readonly CacheDatabaseManager _db;

        public ActionController(CacheDatabaseManager db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AllItemsResult<ActionResult>), 200)]
        public IActionResult GetAllActions()
        {
            var query = _db.Database.GetCollection<Action>(ResourceType.Action.Name).AsQueryable();
            var userId = User.Identity.GetUserIdentity();
            var result = query.Where(x => x.UserId == userId).ToList()
                              .Select(x => x.CreateActionResult())
                              .ToList();
            return Ok(new AllItemsResult<ActionResult>() { Total = result.Count, Items = result });
        }
    }
}
