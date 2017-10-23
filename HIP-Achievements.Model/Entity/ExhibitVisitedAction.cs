using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;

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
    }
}
