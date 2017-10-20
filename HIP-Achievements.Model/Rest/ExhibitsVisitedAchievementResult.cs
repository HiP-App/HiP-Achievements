using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    class ExhibitsVisitedAchievementResult : AchievementResult
    {
        public int Count { get; set; }
        public ExhibitsVisitedAchievementResult(ExhibitsVisitedAchievement achievement) : base(achievement)
        {
            Count = achievement.Count;
        }
    }
}
