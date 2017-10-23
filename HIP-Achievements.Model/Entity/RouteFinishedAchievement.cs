using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class RouteFinishedAchievement : Achievement
    {
        public RouteFinishedAchievement(AchievementArgs args) : base(args)
        {

        }

        public override string TypeName => "RouteFinished";
        public override AchievementResult CreateAchievementResult()
        {
            return new RouteFinishedAchievementResult(this);
        }
    }
}