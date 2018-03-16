using System.Collections.Generic;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Achievements.Core;

namespace PaderbornUniversity.SILab.Hip.Achievements.Tests
{
    internal class FakeRouteClient : IRoutesClient
    {
        public Task<IEnumerable<RouteResult>> GetRoutes()
        {
            return Task.FromResult(new List<RouteResult>() as IEnumerable<RouteResult>);
        }

        public Task<bool> ValidateRouteId(int id)
        {
            return Task.FromResult(true);
        }
    }
}