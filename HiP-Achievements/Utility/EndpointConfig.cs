namespace PaderbornUniversity.SILab.Hip.Achievements.Utility
{
    public class EndpointConfig
    {
        /// <summary>
        /// Connection string for the Mongo DB cache database.
        /// Example: "mongodb://localhost:27017"
        /// </summary>
        public string MongoDbHost { get; set; }

        /// <summary>
        /// Name of the database to use.
        /// Example: "main"
        /// </summary>
        public string MongoDbName { get; set; }
        
        /// <summary>
        /// URL that points to the "swagger.json" file. If set, this URL is entered by default
        /// when accessing the Swagger UI page. If not set, we will try to construct the URL
        /// automatically which might result in an invalid URL.
        /// </summary>
        public string SwaggerEndpoint { get; set; }

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
