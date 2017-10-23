using System.ComponentModel;
using System.Runtime.Serialization;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{

    public class AchievementQueryArgs : QueryArgs
    {
        public string TypeName { get; set; }

        [DefaultValue(AchievementQueryStatus.All)]
        public AchievementQueryStatus Status { get; set; } = AchievementQueryStatus.All;
    }


    public enum AchievementQueryStatus
    {
        [EnumMember(Value = "DRAFT")]
        Draft,
        [EnumMember(Value = "IN_REVIEW")]
        In_Review,
        [EnumMember(Value = "PUBLISHED")]
        Published,
        [EnumMember(Value = "ALL")]
        All
    }

}
