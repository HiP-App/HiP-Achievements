namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public class EndpointConfig
    {
        /// <summary>
        /// Connection string for the Mongo DB cache database.
        /// Default value: "mongodb://localhost:27017"
        /// </summary>
        public string MongoDbHost { get; set; } = "mongodb://localhost:27017";

        /// <summary>
        /// Name of the database to use.
        /// Default value: "main"
        /// </summary>
        public string MongoDbName { get; set; } = "main";
        
        /// <remarks>
        /// This property is optional: If no value is provided, no thumbnail URLs are generated - instead, direct URLs
        /// to the original image files are then returned.
        /// </remarks>
        public string ThumbnailUrlPattern { get; set; }

        /// <summary>
        /// Endpoint of the Datastore
        /// </summary>
        public string DataStoreHost { get; set; }

        /// <summary>
        /// Endpoint of the ThumbnailService
        /// </summary>
        public string ThumbnailServiceHost { get; set; }
    }
}
