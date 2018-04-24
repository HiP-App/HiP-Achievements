using PaderbornUniversity.SILab.Hip.EventSourcing;
using System;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public abstract class ContentBase : IEntity<int>
    {
        public int Id { get; set; }
        /// <summary>
        /// Owner of the content
        /// </summary>
        public string UserId { get; set; }

        public string LastModifiedBy { get; set; } 

        /// <summary>
        /// The date and time of the last modification.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
     }
}
