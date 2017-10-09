using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ExampleController : Controller
    {
        private readonly EventStoreClient _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;
 
        
        public ExampleController(EventStoreClient eventStore, CacheDatabaseManager db, InMemoryCache cache)
        {
            _eventStore = eventStore;
            _db = db;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok();
        }     

    }
}
