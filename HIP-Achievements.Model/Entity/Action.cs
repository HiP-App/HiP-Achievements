﻿using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public abstract class Action : ContentBase
    {
        public abstract string TypeName { get; }

        /// <summary>
        /// Id of entity, which was completed
        /// </summary>
        public int EntityId { get; set; }

        public Action(ActionArgs args)
        {
            EntityId = args.EntityId;
        }

        public abstract ActionResult CreateActionResult();

        public abstract ActionArgs CreateActionArgs();

    }
}
