using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class ExhibitsVisistedArgs : IAchievementTypeArgs
    {
        [JsonProperty("count", Required = Required.Always)]
        public int Count { get; set; }
    }
}
