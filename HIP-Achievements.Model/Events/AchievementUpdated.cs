using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementUpdated : UserActivityBaseEvent, IUpdateEvent
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public AchievementArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceType.Achievement;
    }
}
