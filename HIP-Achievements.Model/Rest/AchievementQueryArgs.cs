using System.Runtime.Serialization;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class AchievementQueryArgs
    {

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
