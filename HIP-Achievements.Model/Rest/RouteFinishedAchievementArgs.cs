using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class RouteFinishedAchievementArgs : AchievementArgs
    {
        public override Achievement CreateAchievement()
        {
            return new RouteFinishedAchievement(this);
        }
    }
}
