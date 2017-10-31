using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions
{
    public class ExhibitVisitedActionArgs : ActionArgs
    {
        public override Action CreateAction() => new ExhibitVisitedAction(this);
    }
    public class ExhibitVisitedActionsArgs : ActionsArgs
    {
        public override List<Action> CreateActions() => ExhibitVisitedAction.Factory(this);

        public override List<ActionArgs> ToListActionArgs()
        { 
            var result = new List<ActionArgs>();
            foreach (var entityId in EntitiesId)
            {
               result.Add(new ExhibitVisitedActionArgs() { EntityId = entityId });
            }
            return result;
        }
    }
}
