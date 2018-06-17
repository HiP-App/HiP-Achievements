using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Achievements;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest.Actions;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Migrations;

namespace PaderbornUniversity.SILab.Hip.Achievements.Migrations
{
    [StreamMigration(from: 1, to: 2)]
    public class Migration2PropertyChanged : IStreamMigration
    {
        private Dictionary<(ResourceType, int), object> argumentDictionary = new Dictionary<(ResourceType, int), object>();
        public async Task MigrateAsync(IStreamMigrationArgs e)
        {
            var events = e.GetExistingEvents();
            while (await events.MoveNextAsync())
            {
                IEnumerable<PropertyChangedEvent> propEvents = new List<PropertyChangedEvent>();
                DateTimeOffset timestamp=new DateTimeOffset();
                switch (events.Current)
                {
                    case AchievementCreated ev:
                        timestamp = ev.Timestamp;
                        argumentDictionary[(ev.GetEntityType(), ev.Id)] = ev.Properties;
                        switch (ev.Properties)
                        {
                            case ExhibitsVisitedAchievementArgs args:
                                e.AppendEvent(new CreatedEvent(ResourceTypes.ExhibitsVisitedAchievement.Name, ev.Id, ev.UserId));
                                propEvents = EntityManager.CompareEntities(new ExhibitsVisitedAchievementArgs(), 
                                                                           args, ResourceTypes.ExhibitsVisitedAchievement, ev.Id, ev.UserId);
                                break;

                            case RouteFinishedAchievementArgs args:
                                e.AppendEvent(new CreatedEvent(ResourceTypes.RouteFinishedAchievement.Name, ev.Id, ev.UserId));
                                propEvents = EntityManager.CompareEntities(new RouteFinishedAchievementArgs(), 
                                                                           args, ResourceTypes.RouteFinishedAchievement, ev.Id, ev.UserId);
                                break;
                        }

                        break;

                    case ActionCreated ev:
                        timestamp = ev.Timestamp;
                        argumentDictionary[(ev.GetEntityType(), ev.Id)] = ev.Properties;
                        switch (ev.Properties)
                        {
                            case ExhibitVisitedActionArgs args:
                                e.AppendEvent(new CreatedEvent(ResourceTypes.ExhibitVisitedAction.Name, ev.Id, ev.UserId));
                                propEvents = EntityManager.CompareEntities(new ExhibitVisitedActionArgs(), 
                                                                           args, ResourceTypes.ExhibitVisitedAction, ev.Id, ev.UserId);
                                break;

                        }
                        break;

                    case AchievementUpdated ev:
                        timestamp = ev.Timestamp;
                        switch (ev.Properties)
                        {
                            case ExhibitsVisitedAchievementArgs args:
                                propEvents = EntityManager.CompareEntities((ExhibitsVisitedAchievementArgs)argumentDictionary[(ev.GetEntityType(), ev.Id)], 
                                                                           args, ResourceTypes.ExhibitsVisitedAchievement, ev.Id, ev.UserId);
                                break;

                            case RouteFinishedAchievementArgs args:
                                propEvents = EntityManager.CompareEntities((RouteFinishedAchievementArgs)argumentDictionary[(ev.GetEntityType(), ev.Id)], 
                                                                           args, ResourceTypes.RouteFinishedAchievement, ev.Id, ev.UserId);
                                break;
                        }
                        argumentDictionary[(ev.GetEntityType(), ev.Id)] = ev.Properties;
                        break;

                    case AchievementDeleted ev:
                        e.AppendEvent(new DeletedEvent(ev.GetEntityType().Name, ev.Id, ev.UserId)
                        {
                            Timestamp = ev.Timestamp
                        });
                        break;


                    default:
                        e.AppendEvent(events.Current);
                        break;
                }

                foreach (var propEvent in propEvents)
                {
                    propEvent.Timestamp = timestamp;
                    e.AppendEvent(propEvent);
                }
            }
        }
    }
}
