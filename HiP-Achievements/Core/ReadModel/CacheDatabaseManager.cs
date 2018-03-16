using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo;
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
        private readonly IMongoDbContext _db;

        public CacheDatabaseManager(
            EventStoreService eventStore,
            IMongoDbContext db)
        {
            // For now, the cache database is always created from scratch by replaying all events.
            // This also implies that, for now, the cache database always contains the entire data (not a subset).
            // In order to receive all the events, a Catch-Up Subscription is created.
            _db = db;

            // Subscribe to EventStore to receive all past and future events
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
                            _db.Add(ResourceTypes.Achievement, newAchievement);
                            break;
                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Action:
                            var actionArgs = (ActionArgs)Activator.CreateInstance(resourceType.Type, true);
                            var newAction = actionArgs.CreateAction();
                            newAction.Id = e.Id;
                            newAction.UserId = e.UserId;
                            newAction.LastModifiedBy = e.UserId;
                            newAction.Timestamp = e.Timestamp;
                            _db.Add(ResourceTypes.Action, newAction);
                            break;

                    }
                    break;

                case PropertyChangedEvent e:
                    resourceType = e.GetEntityType();

                    switch (resourceType)
                    {
                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Achievement:
                            var originalAchievement = _db.GetCollection<Achievement>(ResourceTypes.Achievement).AsQueryable().First(a => a.Id == e.Id);
                            var achievementArgs = originalAchievement.CreateAchievementArgs();
                            e.ApplyTo(achievementArgs);
                            var updatedAchievement = (Achievement)Activator.CreateInstance(originalAchievement.GetType(), achievementArgs);
                            updatedAchievement.LastModifiedBy = e.UserId;
                            updatedAchievement.Timestamp = e.Timestamp;
                            updatedAchievement.UserId = originalAchievement.UserId;
                            updatedAchievement.Id = e.Id;
                            updatedAchievement.Filename = originalAchievement.Filename;
                            _db.Replace((ResourceTypes.Achievement, e.Id), updatedAchievement);
                            break;

                        case ResourceType _ when resourceType.BaseResourceType == ResourceTypes.Action:
                            var originalAction = _db.GetCollection<Action>(ResourceTypes.Action).AsQueryable().First(a => a.Id == e.Id);
                            var actionArgs = originalAction.CreateActionArgs();
                            e.ApplyTo(actionArgs);
                            var updatedAction = (Action)Activator.CreateInstance(originalAction.GetType(), actionArgs);
                            updatedAction.LastModifiedBy = e.UserId;
                            updatedAction.Timestamp = e.Timestamp;
                            updatedAction.UserId = originalAction.UserId;
                            updatedAction.Id = originalAction.Id;
                            _db.Replace((ResourceTypes.Action, e.Id), updatedAction);
                            break;
                    }
                    break;

                case DeletedEvent e:
                    _db.Delete((ResourceTypes.Achievement, e.Id));
                    break;

                case AchievementImageUpdated e:
                    var achievement = _db.GetCollection<Achievement>(ResourceTypes.Achievement).AsQueryable()
                        .FirstOrDefault(a => a.Id == e.Id);
                    if (achievement != null)
                    {
                        achievement.Filename = e.File;
                        _db.Replace((ResourceTypes.Achievement, e.Id), achievement);
                    }
                    break;


            }

        }
    }
}
