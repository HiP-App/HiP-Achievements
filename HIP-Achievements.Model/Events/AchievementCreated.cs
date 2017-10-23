using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Events
{
    public class AchievementCreated : UserActivityBaseEvent, ICreateEvent
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public AchievementArgs Properties { get; set; }

        public override ResourceType GetEntityType() => ResourceType.Achievement;

    }
}
