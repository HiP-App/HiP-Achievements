using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.ActionControllers
{
    public class ExhibitVisitedController : ActionBaseController<ExhibitVisitedActionArgs>
    {
        private readonly EndpointConfig _endpointConfig;
        private readonly ExhibitsVisitedIndex _index;

        public ExhibitVisitedController(EventStoreClient eventStore, InMemoryCache cache, IOptions<EndpointConfig> endpointConfig) : base(eventStore, cache)
        {
            _endpointConfig = endpointConfig.Value;
            _index = cache.Index<ExhibitsVisitedIndex>();
        }

        protected override async Task<ArgsValidationResult> ValidateActionArgs(ActionArgs args)
        {

            //check if the user has visited the exhibit already
            if (_index.Exists(User.Identity.GetUserIdentity(), args.EntityId))
            {
                return new ArgsValidationResult { ActionResult = BadRequest(new { Message = "The user has already visited this exhibit" }) };
            }

            //check if exhibits exists
            var client = new ExhibitsClient(_endpointConfig.DataStoreHost)
            {
                Authorization = Request.Headers["Authorization"]
            };

            try
            {
                //this method throws a SwaggerException if the request fails 
                await client.GetByIdAsync(args.EntityId, null);
                return new ArgsValidationResult { Success = true };
            }
            catch (SwaggerException)
            {
                return new ArgsValidationResult { ActionResult = NotFound(new { Message = "An exhibit with this id doesn't exist" }), Success = false };
            }

        }
    }
}
