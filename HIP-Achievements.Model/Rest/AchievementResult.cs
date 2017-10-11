using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class AchievementResult
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AchievementType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Image { get; set; }

        public int? NextId { get; set; }

        public AchievementResult()
        {
        }

        public AchievementResult(Achievement a)
        {
            Id = a.Id;
            Type = a.Type;
            Status = a.Status;
            Title = a.Title;
            Description = a.Description;
            Image = a.Image;
            NextId = a.NextId;
        }
    }
}
