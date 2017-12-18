using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.AchievementControllers
{
    public class ExhibitsVisitedController : AchievementBaseController<ExhibitsVisitedAchievementArgs>
    {
        public ExhibitsVisitedController(EventStoreService eventStore, InMemoryCache cache) : base(eventStore, cache)
        {
        }

        protected override ResourceType ResourceType => ResourceTypes.ExhibitsVisitedAchievement;

        protected override Task<ArgsValidationResult> ValidateActionArgs(ExhibitsVisitedAchievementArgs args)
        {
            return Task.FromResult(new ArgsValidationResult { Success = true });
        }
    }
}