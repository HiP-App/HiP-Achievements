using System;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using System.Linq;
using Action = PaderbornUniversity.SILab.Hip.Achievements.Model.Entity.Action;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel
{
    /// <summary>
    /// Subscribes to EventStore events to keep the cache database up to date.
    /// </summary>
    public class CacheDatabaseManager
    {
        private readonly EventStoreClient _eventStore;
        private readonly IMongoDatabase _db;

        public IMongoDatabase Database => _db;

        public CacheDatabaseManager(
            EventStoreClient eventStore,
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

            var subscription = _eventStore.EventStream.SubscribeCatchUp();
            subscription.EventAppeared.Subscribe(ApplyEvent);
        }

        private void ApplyEvent(IEvent ev)
        {
            switch (ev)
            {
                case AchievementCreated e:
                    var newAchievement = new Achievement(e.Properties)
                    {
                        Id = e.Id,
                        UserId = e.UserId,
                        LastModifiedBy = e.UserId,
                        Timestamp = e.Timestamp
                    };

                    _db.GetCollection<Achievement>(ResourceType.Achievement.Name).InsertOne(newAchievement);
                    break;

                case AchievementDeleted e:
                    _db.GetCollection<Achievement>(ResourceType.Achievement.Name).DeleteOne(a => a.Id == e.Id);
                    break;
                case AchievementUpdated e:
                    var originalAchievement = _db.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable().First(a => a.Id == e.Id);
                    var updatedAchievement = new Achievement(e.Properties)
                    {
                        Timestamp = e.Timestamp,
                        UserId = originalAchievement.UserId,
                        LastModifiedBy = e.UserId,
                        Id = e.Id
                    };
                    _db.GetCollection<Achievement>(ResourceType.Achievement.Name).ReplaceOne(a => a.Id == e.Id, updatedAchievement);
                    break;

                case ActionCreated e:
                    var newAction = new Action(e.Properties)
                    {
                        Id = e.Id,
                        UserId = e.UserId,
                        LastModifiedBy = e.UserId,
                        Timestamp = e.Timestamp
                    };
                    _db.GetCollection<Action>(ResourceType.Action.Name).InsertOne(newAction);
                    break;

                case AchievementImageUpdated e:
                    var achievement = _db.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable()
                        .FirstOrDefault(a => a.Id == e.Id);
                    achievement.Filename = e.File;
                    _db.GetCollection<Achievement>(ResourceType.Achievement.Name).ReplaceOne(a => a.Id == e.Id, achievement);

                    break;
            }

        }
    }
}
