using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Migrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Migrations
{
    [StreamMigration(from: 2, to: 3)]
    public class Migration3FixDeleteAchievement : IStreamMigration
    {
        private List<int> _deletedIds = new List<int>();

        public async Task MigrateAsync(IStreamMigrationArgs e)
        {
            var events = e.GetExistingEvents();
            while (await events.MoveNextAsync())
            {
                switch (events.Current)
                {
                    case AchievementDeleted ev:
                        if (!_deletedIds.Contains(ev.Id))
                        {
                            e.AppendEvent(new DeletedEvent(ev.GetEntityType().Name, ev.Id, ev.UserId)
                            {
                                Timestamp = ev.Timestamp
                            });
                            _deletedIds.Add(ev.Id);
                        }
                        break;

                    default:
                        e.AppendEvent(events.Current);
                        break;
                }
            }
        }
    }
}
