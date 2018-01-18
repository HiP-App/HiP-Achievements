namespace PaderbornUniversity.SILab.Hip.Achievements
{
    /// <summary>
    /// Configuration properties for clients using the Achievements SDK.
    /// </summary>
    public sealed class AchievementsConfig
    {
        /// <summary>
        /// URL pointing to a running instance of the Achievements service.
        /// Example: "https://docker-hip.cs.upb.de/develop/achievements"
        /// </summary>
        public string AchievementsHost { get; set; }
    }
}
