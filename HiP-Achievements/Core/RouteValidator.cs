using System.Collections.Generic;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.DataStore;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Achievements.Core
{
    public class RoutesClient : IRoutesClient
    {
        private DataStoreService _service;

        public RoutesClient(DataStoreService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<RouteResult>> GetRoutes()
        {
            var routes = await _service.Routes.GetAsync();
            return routes.Items.Select(r => new RouteResult() { RouteId = r.Id, ExhibitIds = r.Exhibits }).ToList();
        }

        public async Task<bool> ValidateRouteId(int id)
        {
            try
            {
                // ReSharper disable once PossibleInvalidOperationException
                await _service.Routes.GetByIdAsync(id);
                return true;
            }
            catch (SwaggerException)
            {
                return false;
            }
        }
    }
}
