using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using Action = PaderbornUniversity.SILab.Hip.Achievements.Model.Entity.Action;
using System;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class ActionResult
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type { get; set; }

        public int EntityId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public ActionResult(Action action)
        {
            Id = action.Id;
            UserId = action.UserId;
            Type = action.Type;
            EntityId = action.EntityId;
            Timestamp = action.Timestamp;
        }
    }
}
