using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class Achievement : ContentBase
    {
        public AchievementType Type { get; set; }

        public IAchievementTypeArgs TypeArgs { get; set; }

        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Image { get; set; }

        public int? NextId { get; set; }

        public Achievement()
        {
        }

        public Achievement(AchievementArgs args, IAchievementTypeArgs typeArgs)
        {
            Type = args.Type;
            Status = args.Status;
            Description = args.Description;
            NextId = args.NextId;
            Title = args.Title;
            TypeArgs = typeArgs;
        }
    }

    /// <remark>
    ///  If Changed, add to <see cref="AchievementQueryStatus"/> 
    /// </remark>
    public enum AchievementStatus
    {
        Unpublished, Published
    }

    public enum AchievementType
    {
        ExhibitVisited, RouteFinished
    }

    public interface IAchievementTypeArgs { }
}
