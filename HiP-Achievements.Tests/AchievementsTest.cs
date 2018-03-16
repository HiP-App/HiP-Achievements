using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.FakeStore;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo.Test;
using System.Threading.Tasks;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.Achievements.Tests
{

    public class AchievementsTest
    {
        private readonly TestServer _server;
        private readonly FakeEventStore _eventStore;
        private readonly FakeMongoDbContext _mongoDb;

        public AchievementsTest()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            _eventStore = (FakeEventStore)_server.Host.Services.GetService<IEventStore>();
            _mongoDb = (FakeMongoDbContext)_server.Host.Services.GetService<IMongoDbContext>();
        }

        [Fact]
        public async Task Test1()
        {
            var clientForGet = new AchievementsClient("")
            {
                CreateHttpClient = _server.CreateClient,
                Authorization = "Admin-Administrator"
            };

            var exhibitsVisitedClient = new ExhibitsVisitedClient("")
            {
                CreateHttpClient = _server.CreateClient,
                Authorization = "Admin-Administrator"
            };

            var routeFinishedClient = new RouteFinishedClient("")
            {
                CreateHttpClient = _server.CreateClient,
                Authorization = "Admin-Administrator"
            };

            var args = new ExhibitsVisitedAchievementArgs()
            {
                Title = "ExhibitsVisited achievement",
                Description = "This is a ExhibitsVisited achievement",
                Count = 5,
                Points = 1
            };

            var args2 = new RouteFinishedAchievementArgs()
            {
                Title = "RouteFinished achievement",
                Description = "This is a RouteFinished achievement",
                Points = 1,
                RouteId = 0
            };

            var id = await exhibitsVisitedClient.CreateAchievementAsync(args);
            Assert.Equal(6, _eventStore.Streams["test"].Events.Count);

            var id2 = await routeFinishedClient.CreateAchievementAsync(args2);
            Assert.Equal(12, _eventStore.Streams["test"].Events.Count);

            var achievements = await clientForGet.GetAllAchievementsAsync(new AchievementQueryArgs() { Status = AchievementQueryStatus.All });

            //test if results has the right types
            Assert.IsType<ExhibitsVisitedAchievementResult>(achievements.Items[0]);
            Assert.IsType<RouteFinishedAchievementResult>(achievements.Items[1]);

            //try to get the exhibits by their id
            var exhibitVisitedAchievement = await clientForGet.GetAchievementByIdAsync(id);
            var routeFinishedAchievement = await clientForGet.GetAchievementByIdAsync(id2);


            //check if the MongoDB was updated correctly
            var cachedExhibitVisitedAchievement = (ExhibitsVisitedAchievement)_mongoDb.Get<Achievement>((ResourceTypes.Achievement, id));

            Assert.Equal(args.Description, cachedExhibitVisitedAchievement.Description);
            Assert.Equal(args.Title, cachedExhibitVisitedAchievement.Title);
            Assert.Equal(args.Points, cachedExhibitVisitedAchievement.Points);
            Assert.Equal(args.Count, cachedExhibitVisitedAchievement.Count);
        }
    }
}
