using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core
{
    public interface IRoutesClient
    {
        Task<bool> ValidateRouteId(int id);
        Task<IEnumerable<RouteResult>> GetRoutes();
    }

    public class RouteResult
    {
        public int RouteId { get; set; }

        public IEnumerable<int> ExhibitIds { get; set; }
    }
}
