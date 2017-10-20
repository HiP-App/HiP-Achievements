using System.Runtime.Serialization;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public abstract class Achievement : ContentBase
    {
        public abstract string TypeName { get; }

        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Filename { get; set; }

        public int? Image { get; set; }

        public int? NextId { get; set; }

        public int Points { get; set; }

        public Achievement()
        {
        }

        public Achievement(AchievementArgs args)
        {
            Status = args.Status;
            Description = args.Description;
            NextId = args.NextId;
            Title = args.Title;
            Points = args.Points;
        }

        public abstract AchievementResult CreateAchievementResult();
    }

    /// <remark>
    ///  If Changed, add to <see cref="AchievementQueryStatus"/> 
    /// </remark>
    public enum AchievementStatus
    {
        [EnumMember(Value = "DRAFT")]
        Draft,
        [EnumMember(Value = "IN_REVIEW")]
        InReview,
        [EnumMember(Value = "PUBLISHED")]
        Published
    }


}
