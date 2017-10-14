using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class ActionCreated : UserActivityBaseEvent, ICreateEvent
    {
        public ActionArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceType.Action;
    }
}
