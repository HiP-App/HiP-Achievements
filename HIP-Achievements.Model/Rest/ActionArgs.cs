using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest
{
    public class ActionArgs
    {
        [Required]
        public ActionType Type { get; set; }

        /// <summary>
        /// Id of entity, which was completed
        /// </summary>
        [Required]
        public int EntityId { get; set; }
    }
}
