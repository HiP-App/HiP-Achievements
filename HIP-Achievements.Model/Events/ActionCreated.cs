using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class ActionCreated : UserActivityBaseEvent, ICreateEvent
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public ActionArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceType.Action;
    }
}
