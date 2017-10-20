using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using System;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    /// <summary>
    /// Model for returning Achievements, derived class is used if additional properties need to be returned 
    /// </summary>
    public abstract class AchievementResult
    {
        public int Id { get; set; }

        public string Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int? NextId { get; set; }

        public int Points { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        
        protected AchievementResult()
        {
        }

        public AchievementResult(Achievement a)
        {
            Id = a.Id;
            Type = a.TypeName;
            Status = a.Status;
            Title = a.Title;
            Description = a.Description;
            NextId = a.NextId;
            UserId = a.UserId;
            Timestamp = a.Timestamp;
            Points = a.Points;
        }
    }
}
