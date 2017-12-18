using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class RouteFinishedAchievement : Achievement
    {
        public int RouteId { get; set; }
        public RouteFinishedAchievement(RouteFinishedAchievementArgs args) : base(args)
        {
            if (args.RouteId != null) RouteId = args.RouteId.Value;
        }

        public RouteFinishedAchievement()
        {
        }


        public override string TypeName => "RouteFinished";
        public override AchievementResult CreateAchievementResult()
        {
            return new RouteFinishedAchievementResult(this);
        }

        public override AchievementArgs CreateAchievementArgs()
        {
            return new RouteFinishedAchievementArgs()
            {
                Description = Description,
                NextId = NextId,
                Points = Points,
                RouteId = RouteId,
                Status = Status,
                Title = Title
            };
        }
    }
}