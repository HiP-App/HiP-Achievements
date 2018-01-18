using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementUpdated : UserActivityBaseEvent, IUpdateEvent
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public AchievementArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceTypes.Achievement;
    }
}
