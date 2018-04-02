using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Migrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Migrations
{
    [StreamMigration(from: 3, to: 4)]
    public class Migration4FixReportedActionIds : IStreamMigration
    {
        public async Task MigrateAsync(IStreamMigrationArgs e)
        {
            var events = e.GetExistingEvents();
            int lastIdSeen = 0;
            int nextId = 0;
            while (await events.MoveNextAsync())
            {
                switch (events.Current)
                {
                    case CreatedEvent ev when ev.GetEntityType() == ResourceTypes.ExhibitVisitedAction:
                        if (ev.Id < lastIdSeen)
                        {
                            ev.Id = nextId;
                        }
                        lastIdSeen = ev.Id;
                        nextId++;
                        e.AppendEvent(ev);
                        break;

                    case PropertyChangedEvent ev when ev.GetEntityType() == ResourceTypes.ExhibitVisitedAction:
                        if (ev.Id < lastIdSeen)
                        {
                            ev.Id = lastIdSeen;
                        }
                        e.AppendEvent(ev);
                        break;

                    default:
                        e.AppendEvent(events.Current);
                        break;
                }
            }
        }
    }
}
