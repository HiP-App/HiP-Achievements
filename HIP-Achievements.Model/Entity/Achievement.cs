using System;
using System.Collections.Generic;
using System.Text;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class Achievement : ContentBase
    {
        public AchievementType Type { get; set; }

        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int NextId { get; set; }

        public Achievement()
        {
        }

        public Achievement(AchievementArgs args)
        {
            Type = args.Type;
            Status = args.Status;
            Description = args.Description;
            NextId = args.NextId;
            Title = args.Title;
        }
    }

    public enum AchievementStatus
    {
        Unpublished, Published
    }

    public enum AchievementType
    {
        ExhibitVisited, RouteFinished
    }
}
