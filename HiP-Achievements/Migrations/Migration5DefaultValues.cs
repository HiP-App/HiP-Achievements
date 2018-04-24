using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Migrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Migrations
{
    [StreamMigration(from: 4, to: 5)]
    public class Migration5DefaultValues : IStreamMigration
    {
        //this list contains the ids where no PropertyChangedEvent was created for the EntityId
        private List<int> relevantIds = new List<int>();

        public async Task MigrateAsync(IStreamMigrationArgs e)
        {
            var events = e.GetExistingEvents();
            while (await events.MoveNextAsync())
            {
                switch (events.Current)
                {
                    case CreatedEvent ev when ev.GetEntityType() == ResourceTypes.ExhibitVisitedAction:
                        relevantIds.Add(ev.Id);
                        break;

                    case PropertyChangedEvent ev when ev.GetEntityType() == ResourceTypes.ExhibitVisitedAction:
                        relevantIds.Remove(ev.Id);
                        break;


                }
            }

            events = e.GetExistingEvents();

            while (await events.MoveNextAsync())
            {
                if (events.Current is CreatedEvent ev && ev.GetEntityType() == ResourceTypes.ExhibitVisitedAction && relevantIds.Contains(ev.Id))
                {
                    e.AppendEvent(events.Current);
                    e.AppendEvent(new PropertyChangedEvent(nameof(ExhibitVisitedAction.EntityId), nameof(ExhibitVisitedAction), ev.Id, ev.UserId, 0));
                }
                else
                {
                    e.AppendEvent(events.Current);
                }
            }

        }
    }
}
