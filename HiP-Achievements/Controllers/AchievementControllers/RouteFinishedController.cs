using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.AchievementControllers
{

    public class RouteFinishedController : AchievementBaseController<RouteFinishedAchievementArgs>
    {
        private readonly EndpointConfig _endpointConfig;

        public RouteFinishedController(EventStoreService eventStore, InMemoryCache cache, IOptions<EndpointConfig> endpointConfig) : base(eventStore, cache)
        {
            _endpointConfig = endpointConfig.Value;
        }

        protected override async Task<ArgsValidationResult> ValidateActionArgs(RouteFinishedAchievementArgs args)
        {
            var client = new RoutesClient(_endpointConfig.DataStoreHost) { Authorization = Request.Headers["Authorization"]};
            try
            {
                // ReSharper disable once PossibleInvalidOperationException
                await client.GetByIdAsync(args.RouteId.Value);
                return new ArgsValidationResult { Success = true };
            }
            catch (SwaggerException)
            {
                return new ArgsValidationResult { ActionResult = NotFound(new { Message = "A route with this id doesn't exist" }), Success = false };
            }
        }
    }
}
