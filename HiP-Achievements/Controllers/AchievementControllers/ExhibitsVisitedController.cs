using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.AchievementControllers
{
    public class ExhibitsVisitedController : AchievementBaseController<ExhibitsVisitedAchievementArgs>
    {
        public ExhibitsVisitedController(EventStoreClient eventStore, InMemoryCache cache) : base(eventStore, cache)
        {
        }

        protected override Task<ArgsValidationResult> ValidateActionArgs(ExhibitsVisitedAchievementArgs args)
        {
            return Task.FromResult(new ArgsValidationResult { Success = true });
        }
    }
}