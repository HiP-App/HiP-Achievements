using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class ExhibitVisitedAction : Action
    {
        public ExhibitVisitedAction(ExhibitVisitedActionArgs args) : base(args)
        {
        }

        public override string TypeName => "ExhibitVisited";
        public override ActionResult CreateActionResult()
        {
            return new ExhibitVisitedActionResult(this);
        }

        public static List<Action> Factory(ExhibitVisitedActionsArgs args)
        {
            var result = new List<Action>();
            foreach (var actionArg in args.ToListActionArgs())
            {
                result.Add(new ExhibitVisitedAction((ExhibitVisitedActionArgs) actionArg));
            }
            return result;         
        }
    }
}
