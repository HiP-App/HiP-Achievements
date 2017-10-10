using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementCreated : UserActivityBaseEvent, ICreateEvent
    {
        public AchievementArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceType.Achievement;

    }
}
