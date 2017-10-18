using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
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

        [Required]
        public JObject TypeArgs { get; set; }

        public AchievementStatus Status { get; set; } = AchievementStatus.Unpublished;


        public string Description { get; set; }

        public int? NextId { get; set; }

        [Range(1, int.MaxValue)]
        public int Points { get; set; }
    }
}