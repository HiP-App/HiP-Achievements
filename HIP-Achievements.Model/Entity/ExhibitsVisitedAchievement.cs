using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class ExhibitsVisitedAchievement : Achievement
    {
        public override string TypeName => "ExhibitsVisited";
        public int Count { get; set; }

        public ExhibitsVisitedAchievement()
        {
        }

        public ExhibitsVisitedAchievement(ExhibitsVisitedAchievementArgs args) : base(args)
        {
            Count = args.Count;
        }

        public override AchievementResult CreateAchievementResult()
        {
            return new ExhibitsVisitedAchievementResult(this);
        }
    }
}
