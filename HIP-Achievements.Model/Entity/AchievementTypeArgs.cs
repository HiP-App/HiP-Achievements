using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class ExhibitsVisistedArgs : IAchievementTypeArgs
    {
        [JsonProperty("count", Required = Required.Always)]
        public int Count { get; set; }
    }
}
