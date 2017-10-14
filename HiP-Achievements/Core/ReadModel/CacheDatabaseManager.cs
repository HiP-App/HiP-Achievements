using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using System;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel
{
    /// <summary>
    /// Subscribes to EventStore events to keep the cache database up to date.
    /// </summary>
    public class CacheDatabaseManager
    {
        private readonly EventStoreService _eventStore;
        private readonly IMongoDatabase _db;

        public IMongoDatabase Database => _db;

        public CacheDatabaseManager(
            EventStoreService eventStore,
            IOptions<EndpointConfig> config,
            ILogger<CacheDatabaseManager> logger)
        {
            // For now, the cache database is always created from scratch by replaying all events.
            // This also implies that, for now, the cache database always contains the entire data (not a subset).
            // In order to receive all the events, a Catch-Up Subscription is created.

            // 1) Open MongoDB connection and clear existing database
            var mongo = new MongoClient(config.Value.MongoDbHost);
            mongo.DropDatabase(config.Value.MongoDbName);
            _db = mongo.GetDatabase(config.Value.MongoDbName);
            var uri = new Uri(config.Value.MongoDbHost);
            logger.LogInformation($"Connected to MongoDB cache database on '{uri.Host}', using database '{config.Value.MongoDbName}'");

            // 2) Subscribe to EventStore to receive all past and future events
            _eventStore = eventStore;

            var subscription = _eventStore.EventStream.SubscribeCatchUp(ApplyEvent);
        }

        private void ApplyEvent(IEvent ev)
        {
            
        }      
    }
}
