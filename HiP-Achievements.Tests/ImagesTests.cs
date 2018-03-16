using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.FakeStore;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo.Test;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PaderbornUniversity.SILab.Hip.Achievements.Tests
{
    public class ImagesTests
    {
        private readonly TestServer _server;

        public const string SampleImagePath = "Assets/Test.png";

        public ImagesTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
        }

        [Fact]
        public async Task Test1()
        {
            var imageClient = new ImageClient("")
            {
                CreateHttpClient = _server.CreateClient,
                Authorization = "Admin-Administrator"
            };

            var exhibitsVisitedClient = new ExhibitsVisitedClient("")
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

            long fileSize;
            var id = await exhibitsVisitedClient.CreateAchievementAsync(args);
            using (var stream = File.OpenRead(SampleImagePath))
            {
                fileSize = stream.Length;
                await imageClient.PutImageByIdAsync(id, new FileParameter(stream, Path.GetFileName(SampleImagePath)));
            }

            var file = await imageClient.GetImageByIdAsync(id);
            //test if the downloaded file has the same length
            using (var memStream = new MemoryStream())
            {
                file.Stream.CopyTo(memStream);
                Assert.Equal(fileSize, memStream.Length);
            }
        }
    }
}
