namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementDeleted : UserActivityBaseEvent, IDeleteEvent
    {
        public override ResourceType GetEntityType() => ResourceType.Achievement;
    }
}
