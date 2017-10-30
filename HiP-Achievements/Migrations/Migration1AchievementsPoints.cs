using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.EventSourcing.Migrations;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Migrations
{
    public class Migration1AchievementsPoints
    { /// <summary>
      /// Achivements Points doesn`t exist in old version, that`s why AchievmentsArgs.Points = 0 to old Achivements
      ///  But by definition , achievement points > 0
      /// </summary>
        [StreamMigration(from: 0, to: 1)]
        public class Migration1PageOrderFeature : IStreamMigration
        {
            public async Task MigrateAsync(IStreamMigrationArgs e)
            {
                var events = e.GetExistingEvents();             
                while (await events.MoveNextAsync())
                {
                    switch (events.Current)
                    {
                        case AchievementCreated ev:
                            ev.Properties.Points = ev.Properties.Points <= 0 ? 1 : ev.Properties.Points;
                            e.AppendEvent(ev);
                            break;

                        default:
                            // all other events remain the same
                            e.AppendEvent(events.Current);
                            break;
                    }
                }
            }

        }
    }
}
