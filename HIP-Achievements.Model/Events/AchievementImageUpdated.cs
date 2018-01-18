using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementImageUpdated : UserActivityBaseEvent, IUpdateEvent
    {
        public string File { get; set; }

        public override ResourceType GetEntityType()
        {
            return ResourceTypes.Achievement;
        }
    }
}
