using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions
{
    public class ExhibitVisitedActionArgs : ActionArgs
    {
        public override Action CreateAction() => new ExhibitVisitedAction(this);
    }
}
