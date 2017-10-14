using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using System.Runtime.Serialization;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class Action : ContentBase
    {
        public ActionType Type { get; set; }

        /// <summary>
        /// Id of entity, which was completed
        /// </summary>
        public int EntityId { get; set; }

        public Action(ActionArgs args)
        {
            Type = args.Type;
            EntityId = args.EntityId;
        }
    }
    public enum ActionType  
    {
        [EnumMember(Value = "ExhibitVisited")]
        ExhibitVisited
    }
}
