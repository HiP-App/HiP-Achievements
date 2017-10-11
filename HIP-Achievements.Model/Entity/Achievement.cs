using System;
using System.Collections.Generic;
using System.Text;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model.Entity
{
    public class Achievement : ContentBase
    {
        public AchievementType Type { get; set; }

        public AchievementStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Image { get; set; }

        public int? NextId { get; set; }
    }

    public enum AchievementStatus
    {
        Published, Unpublished
    }

    public enum AchievementType
    {
        ExhibitVisited, RouteFinished
    }
}
