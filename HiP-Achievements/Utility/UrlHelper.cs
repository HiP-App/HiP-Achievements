namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public static class UrlHelper
    {
        public static string GenerateImageUrl(string thumnailUrlPattern, int id)
        {
            if (!string.IsNullOrWhiteSpace(thumnailUrlPattern))
            {
                // Generate thumbnail URL (if a thumbnail URL pattern is configured)
                return string.Format(thumnailUrlPattern, id);
            }

            return "";
        }
    }
}
