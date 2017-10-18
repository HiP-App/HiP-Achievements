using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using System;

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

        public string ImageUrl { get; set; }

        public int? NextId { get; set; }

        public int Points { get; set; }

        public string UserId { get; set; }

        public IAchievementTypeArgs TypeArgs { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        
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
            NextId = a.NextId;
            UserId = a.UserId;
            Timestamp = a.Timestamp;
            TypeArgs = a.TypeArgs;
            Points = a.Points;
        }
    }
}
