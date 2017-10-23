using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements
{
    public class RouteFinishedAchievementArgs : AchievementArgs
    {
        [Required]
        public int? RouteId { get; set; }

        public override Achievement CreateAchievement()
        {
            return new RouteFinishedAchievement(this);
        }
    }
}
