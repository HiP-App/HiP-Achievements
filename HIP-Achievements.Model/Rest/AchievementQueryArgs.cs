using System.ComponentModel;
using System.Runtime.Serialization;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class AchievementQueryArgs : QueryArgs
    {
        public AchievementType? Type { get; set; }

        [DefaultValue(AchievementQueryStatus.All)]
        public AchievementQueryStatus Status { get; set; } = AchievementQueryStatus.All; 
    }

    public enum AchievementQueryStatus
    {
        [EnumMember(Value = "UNPUBLISHED")]
        Unpublished,

        [EnumMember(Value = "PUBLISHED")]
        Published,

        [EnumMember(Value = "ALL")]
        All
    }
}
