using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using System.ComponentModel.DataAnnotations;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public abstract class ActionArgs
    {
        /// <summary>
        /// Id of entity, which was completed
        /// </summary>
        [Required]
        public int EntityId { get; set; }

        public abstract Action CreateAction();
    }
}
