using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using System;
using System.Linq;
using Action = PaderbornUniversity.SILab.Hip.Achievements.Model.Entity.Action;

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
            _eventStore.EventStream.SubscribeCatchUp(ApplyEvent);
        }

        private void ApplyEvent(IEvent ev)
        {
            switch (ev)
            {
                case CreatedEvent e:
                    var resourceType = e.GetEntityType();
                    switch (resourceType)
                    {
                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Achievement:
                            var achievementArgs = (AchievementArgs)Activator.CreateInstance(resourceType.Type, true);
                            var newAchievement = achievementArgs.CreateAchievement();
                            newAchievement.Id = e.Id;
                            newAchievement.UserId = e.UserId;
                            newAchievement.LastModifiedBy = e.UserId;
                            newAchievement.Timestamp = e.Timestamp;
                            _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).InsertOne(newAchievement);
                            break;
                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Action:
                            var actionArgs = (ActionArgs)Activator.CreateInstance(resourceType.Type, true);
                            var newAction = actionArgs.CreateAction();
                            newAction.Id = e.Id;
                            newAction.UserId = e.UserId;
                            newAction.LastModifiedBy = e.UserId;
                            newAction.Timestamp = e.Timestamp;
                            _db.GetCollection<Action>(ResourceTypes.Action.Name).InsertOne(newAction);
                            break;

                    }
                    break;

                case PropertyChangedEvent e:
                    resourceType = e.GetEntityType();

                    switch (resourceType)
                    {
                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Achievement:
                            var originalAchievement = _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable().First(a => a.Id == e.Id);
                            var achievementArgs = originalAchievement.CreateAchievementArgs();
                            e.ApplyTo(achievementArgs);
                            var updatedAchievement = (Achievement)Activator.CreateInstance(originalAchievement.GetType(), achievementArgs);
                            updatedAchievement.LastModifiedBy = e.UserId;
                            updatedAchievement.Timestamp = e.Timestamp;
                            updatedAchievement.UserId = originalAchievement.UserId;
                            updatedAchievement.Id = e.Id;
                            updatedAchievement.Filename = originalAchievement.Filename;
                            _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).ReplaceOne(a => a.Id == e.Id, updatedAchievement);
                            break;

                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Action:
                            var originalAction = _db.GetCollection<Action>(ResourceTypes.Action.Name).AsQueryable().First(a => a.Id == e.Id);
                            var actionArgs = originalAction.CreateActionArgs();
                            e.ApplyTo(actionArgs);
                            var updatedAction = (Action)Activator.CreateInstance(originalAction.GetType(), actionArgs);
                            updatedAction.LastModifiedBy = e.UserId;
                            updatedAction.Timestamp = e.Timestamp;
                            updatedAction.UserId = originalAction.UserId;
                            updatedAction.Id = originalAction.Id;
                            _db.GetCollection<Action>(ResourceTypes.Action.Name).ReplaceOne(a => a.Id == e.Id, updatedAction);
                            break;
                    }
                    break;

                case DeletedEvent e:
                    _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).DeleteOne(a => a.Id == e.Id);
                    break;

                case AchievementImageUpdated e:
                    var achievement = _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable()
                        .FirstOrDefault(a => a.Id == e.Id);
                    if (achievement != null)
                    {
                        achievement.Filename = e.File;
                        _db.GetCollection<Achievement>(ResourceTypes.Achievement.Name).ReplaceOne(a => a.Id == e.Id, achievement);
                    }
                    break;


            }

        }
    }
}
