using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementDeleted : UserActivityBaseEvent, IDeleteEvent
    {
        public override ResourceType GetEntityType() => ResourceTypes.Achievement;
    }
}
