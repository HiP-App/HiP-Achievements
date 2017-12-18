using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public class UploadFilesConfig
    {
        /// <summary>
        /// Path to the directory where image files are stored.
        /// Default value: "Images"
        /// </summary>
        public string Path { get; set; } = "Images";

        /// <summary>
        /// A list of supported file extensions for images.
        /// </summary>
        public List<string> SupportedFormats { get; set; }
    }
}
