using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PaderbornUniversity.SILab.Hip.Achievements
{
    /// <summary>
    /// A service that can be used with ASP.NET Core dependency injection.
    /// Usage: In ConfigureServices():
    /// <code>
    /// services.Configure&lt;AchievementsConfig&gt;(Configuration.GetSection("Endpoints"));
    /// services.AddSingleton&lt;AchievementsService&gt;();
    /// </code>
    /// </summary>
    public class AchievementsService
    {
        private readonly AchievementsConfig _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AchievementsService(IOptions<AchievementsConfig> config, ILogger<AchievementsService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _config = config.Value;
            _httpContextAccessor = httpContextAccessor;

            if (string.IsNullOrWhiteSpace(config.Value.AchievementsHost))
                logger.LogWarning($"{nameof(AchievementsConfig.AchievementsHost)} is not configured correctly!");
        }

        public ExhibitsVisitedClient ExhibitsVisited => new ExhibitsVisitedClient(_config.AchievementsHost)
        {
            Authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
        };

        public RouteFinishedClient RouteFinished => new RouteFinishedClient(_config.AchievementsHost)
        {
            Authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
        };

        public ExhibitVisitedClient ExhibitVisisted => new ExhibitVisitedClient(_config.AchievementsHost)
        {
            Authorization = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
        };        
    }
}
