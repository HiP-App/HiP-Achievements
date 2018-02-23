using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private readonly EventStoreService _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;
        private readonly DataStoreService _dataStoreService;
        private readonly UserStoreService _userStoreService;
        private readonly ThumbnailService.ThumbnailService _thumbnailService;

        public AchievementsController(EventStoreService eventStore, CacheDatabaseManager db,
            InMemoryCache cache, DataStoreService dataStoreService, UserStoreService userStoreService,
            ThumbnailService.ThumbnailService thumbnailService)
        {
            _eventStore = eventStore;
            _db = db;
            _dataStoreService = dataStoreService;
            _thumbnailService = thumbnailService;
            _userStoreService = userStoreService;
            _entityIndex = cache.Index<EntityIndex>();
        }


        [HttpGet("ids")]
        [ProducesResponseType(typeof(IEnumerable<int>), 200)]
        public IActionResult GetAchievementIds(AchievementQueryStatus status = AchievementQueryStatus.Published)
        {
            bool isAllowedGetAll = UserPermissions.IsAllowedToGetAll(User.Identity, status);
            var userIdendity = User.Identity.GetUserIdentity();

            Enum.TryParse<AchievementStatus>(status.ToString(), out var achievementStatus);
            var query = _db.Database.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable();
            var achievements = query
                .FilterIf(!isAllowedGetAll, x =>
                    (status == AchievementQueryStatus.All && x.Status == AchievementStatus.Published) ||
                    x.UserId == userIdendity)
                .FilterIf(status != AchievementQueryStatus.All, x => x.Status == achievementStatus)
                .Select(x => x.Id)
                .ToList();

            return Ok(achievements);
        }

        [HttpGet]
        [ProducesResponseType(typeof(AllItemsResult<AchievementResult>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAllAchievements(AchievementQueryArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievements = _db.Database.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable();

            var query = achievements
                   .FilterByIds(args.Exclude, args.IncludeOnly)
                   .FilterByUser(args.Status, User.Identity)
                   .FilterByStatus(args.Status)
                   .FilterByTimestamp(args.Timestamp)
                   .FilterIf(!string.IsNullOrEmpty(args.Query), x =>
                       x.Title.ToLower().Contains(args.Query.ToLower()) ||
                       x.Description.ToLower().Contains(args.Query.ToLower()))
                   .Sort(args.OrderBy,
                       ("id", x => x.Id),
                       ("title", x => x.Title),
                       ("timestamp", x => x.Timestamp)).ToList();

            //MongoDB doesn't support querying on abstract properties, thus we filter for the TypeName seperately
            var result = query.AsQueryable()
                .FilterIf(!string.IsNullOrEmpty(args.TypeName), x => x.TypeName == args.TypeName)
                .PaginateAndSelect(args.Page, args.PageSize, x =>
                {
                    var ar = x.CreateAchievementResult();
                    if (!string.IsNullOrEmpty(x.Filename))
                        ar.ThumbnailUrl = _thumbnailService.GetThumbnailUrlArgument(x.Id);
                    return ar;
                });

            if (result == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetAchievementById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var achievement = _db.Database.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable().FirstOrDefault(a => a.Id == id);

            if (achievement == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            if (!UserPermissions.IsAllowedToGet(User.Identity, achievement.Status, achievement.UserId))
                return Forbid();

            var result = achievement.CreateAchievementResult();
            if (!string.IsNullOrEmpty(achievement.Filename))
                result.ThumbnailUrl = _thumbnailService.GetThumbnailUrlArgument(id);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_entityIndex.Exists(ResourceTypes.Achievement, id))
                return NotFound();

            var achievement = _db.Database.GetCollection<Achievement>(ResourceTypes.Achievement.Name).AsQueryable().First(a => a.Id == id);

            if (!UserPermissions.IsAllowedToDelete(User.Identity, achievement.Status, achievement.UserId))
                return Forbid();

            if (!string.IsNullOrEmpty(achievement.Filename))
            {
                if (System.IO.File.Exists(achievement.Filename))
                    System.IO.File.Delete(achievement.Filename);
                await _thumbnailService.TryClearThumbnailCacheAsync(id);
            }

            //we use the BaseResourceType here
            await EntityManager.DeleteEntityAsync(_eventStore, ResourceTypes.Achievement, id, User.Identity.GetUserIdentity());
            return NoContent();
        }

        [HttpGet("types")]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult Types() => Ok(GetAllTypeNames());

        [HttpGet("Unlocked")]
        [ProducesResponseType(typeof(AllItemsResult<AchievementResult>), 200)]
        [ProducesResponseType(400)]

        public async Task<IActionResult> GetUnlocked()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievements = _db.Database.GetCollection<Achievement>(ResourceTypes.Achievement.Name)
                                          .AsQueryable()
                                          .FilterByStatus(AchievementQueryStatus.Published);

            var actions = await _userStoreService.Actions.GetAllActionsAsync();
            var exhibitVisitedActions = actions.Items.Where(x => x.Type == "ExhibitVisited");

            var unlocked = new List<Achievement>();
            var routes = await _dataStoreService.Routes.GetAsync();

            foreach (var achievement in achievements)
            {
                switch (achievement)
                {
                    case ExhibitsVisitedAchievement e:
                        if (e.Count <= exhibitVisitedActions.ToList().Count)
                        {
                            unlocked.Add(e);
                        }
                        break;


                    case RouteFinishedAchievement e:
                        var visitedExhibitsIds = exhibitVisitedActions.Select(x => x.EntityId).ToList();
                        if (routes.Items.Any(r => r.Id == e.RouteId && r.Exhibits.IsSubsetOf(visitedExhibitsIds)))
                        {
                            unlocked.Add(e);
                        }
                        break;

                }
            }
            var result = new AllItemsResult<AchievementResult>
            {
                Total = unlocked.Count,
                Items = unlocked.Select(x => x.CreateAchievementResult()).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// This method iterates over all classes that inherit from <see cref="Achievement"/> and selects their TypeNames
        /// </summary>
        /// <remarks>
        /// This only works if the derived class has a parameterless constructor, so that an instance can be created
        /// </remarks>
        /// <returns>All type names</returns>
        private IEnumerable<string> GetAllTypeNames()
        {
            return typeof(Achievement)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Achievement)) && !t.IsAbstract)
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => (Achievement)Activator.CreateInstance(t)).Select(a => a.TypeName);
        }
    }
}
