using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.AchievementControllers
{

    public class RouteFinishedController : AchievementBaseController<RouteFinishedAchievementArgs>
    {
        private readonly Core.IRoutesClient _routeValidator;

        public RouteFinishedController(EventStoreService eventStore, InMemoryCache cache, Core.IRoutesClient routeValidator) : base(eventStore, cache)
        {
            _routeValidator = routeValidator;
        }

        protected override ResourceType ResourceType => ResourceTypes.RouteFinishedAchievement;

        protected override async Task<ArgsValidationResult> ValidateActionArgs(RouteFinishedAchievementArgs args)
        {
            var success = await _routeValidator.ValidateRouteId(args.RouteId.Value);
            if (success)
            {
                return new ArgsValidationResult { Success = true };
            }
            else
            {
                return new ArgsValidationResult { ActionResult = NotFound(new { Message = "A route with this id doesn't exist" }), Success = false };
            }
        }
    }
}
