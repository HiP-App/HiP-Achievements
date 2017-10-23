using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements
{
    public class RouteFinishedAchievementResult : AchievementResult
    {
        public int RouteId { get; set; }

        public RouteFinishedAchievementResult(RouteFinishedAchievement achievement) : base(achievement)
        {
            RouteId = achievement.RouteId;
        }
    }
}