﻿using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Model
{
    public static class ResourceTypes
    {
        public static ResourceType Achievement;
        public static ResourceType ExhibitsVisitedAchievement;
        public static ResourceType RouteFinishedAchievement;
        public static ResourceType Action;
        public static ResourceType ExhibitVisitedAction;

        /// <summary>
        /// Initializes the fieldd
        /// </summary>
        public static void Initialize()
        {
            Achievement = ResourceType.Register(nameof(Achievement), typeof(AchievementArgs));
            Action = ResourceType.Register(nameof(Action), typeof(ActionArgs));
            ExhibitsVisitedAchievement = ResourceType.Register(nameof(ExhibitsVisitedAchievement), typeof(ExhibitsVisitedAchievementArgs), Achievement);
            RouteFinishedAchievement = ResourceType.Register(nameof(RouteFinishedAchievement), typeof(RouteFinishedAchievementArgs), Achievement);
            ExhibitVisitedAction = ResourceType.Register(nameof(ExhibitVisitedAction), typeof(ExhibitVisitedActionArgs), Action);
        }
    }
}
