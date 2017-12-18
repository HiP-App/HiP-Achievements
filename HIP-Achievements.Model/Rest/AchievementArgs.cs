using System.ComponentModel.DataAnnotations;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    /// <summary>
    /// Model for creating new Achievements
    /// </summary>
    public abstract class AchievementArgs
    {
        [Required]
        public string Title { get; set; }

        public AchievementStatus Status { get; set; } = AchievementStatus.Draft;

        public string Description { get; set; }

        public int? NextId { get; set; }

        [Range(1, int.MaxValue)]
        public int Points { get; set; }

        public abstract Achievement CreateAchievement();

        public AchievementArgs() { }
    }
}