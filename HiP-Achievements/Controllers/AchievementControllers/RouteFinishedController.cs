using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.AchievementControllers
{

    public class RouteFinishedController : AchievementBaseController<RouteFinishedAchievementArgs>
    {
        private readonly DataStoreService _dataStoreService;

        public RouteFinishedController(EventStoreService eventStore, InMemoryCache cache, DataStoreService dataStoreService) : base(eventStore, cache)
        {
            _dataStoreService = dataStoreService;
        }

        protected override async Task<ArgsValidationResult> ValidateActionArgs(RouteFinishedAchievementArgs args)
        {
            try
            {
                // ReSharper disable once PossibleInvalidOperationException
                await _dataStoreService.Routes.GetByIdAsync(args.RouteId.Value);
                return new ArgsValidationResult { Success = true };
            }
            catch (SwaggerException)
            {
                return new ArgsValidationResult { ActionResult = NotFound(new { Message = "A route with this id doesn't exist" }), Success = false };
            }
        }
    }
}
