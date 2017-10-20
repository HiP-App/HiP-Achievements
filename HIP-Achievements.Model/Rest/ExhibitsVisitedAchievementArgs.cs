using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class ExhibitsVisitedAchievementArgs : AchievementArgs
    {
        [Range(1, int.MaxValue)]
        public int Count { get; set; }
        public override Achievement CreateAchievement()
        {
            return new ExhibitsVisitedAchievement(this);
        }
    }
}
