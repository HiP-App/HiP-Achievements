using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Route("api/[controller]")]
    public class RouteFinishedController : AchievementBaseController<RouteFinishedAchievementArgs>
    {
        public RouteFinishedController(EventStoreClient eventStore, InMemoryCache cache) : base(eventStore, cache)
        {
        }
    }
}
