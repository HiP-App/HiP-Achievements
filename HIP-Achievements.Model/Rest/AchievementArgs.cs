using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    /// <summary>
    /// Model for creating new Achievements
    /// </summary>
    public class AchievementArgs
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public AchievementType Type { get; set; }

        public AchievementStatus Status { get; set; }

        public string Description { get; set; }
        
        public int NextId { get; set; }
    }
}