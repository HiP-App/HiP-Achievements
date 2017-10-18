using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        private readonly EventStoreClient _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly EntityIndex _entityIndex;
        private EndpointConfig _endpointConfig;


        public AchievementsController(EventStoreClient eventStore, CacheDatabaseManager db, InMemoryCache cache, IOptions<EndpointConfig> endpointConfig)
        {
            _eventStore = eventStore;
            _db = db;
            _entityIndex = cache.Index<EntityIndex>();
            _endpointConfig = endpointConfig.Value;
        }

        [HttpGet("ids")]
        [ProducesResponseType(200)]
        public IActionResult GetAllAchievements(AchievementQueryStatus status = AchievementQueryStatus.Published)
        {
            bool isAllowedGetAll = UserPermissions.IsAllowedToGetAll(User.Identity, status);
            var userIdendity = User.Identity.GetUserIdentity();
            Enum.TryParse(typeof(AchievementStatus), status.ToString(), out var achievementStatus);
            var query = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable();
            var achievements = query.FilterIf(!isAllowedGetAll, x =>
            ((status == AchievementQueryStatus.All) && (x.Status == AchievementStatus.Published)) || (x.UserId == userIdendity))
            .FilterIf(status != AchievementQueryStatus.All, x => x.Status == (AchievementStatus)achievementStatus)
            .Select(x => x.Id)
            .ToList();
            return Ok(achievements);
        }

        [HttpGet]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAchievement(AchievementQueryArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievements = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable();

            var result = achievements
                   .FilterByIds(args.Exclude, args.IncludeOnly)
                   .FilterByUser(args.Status, User.Identity)
                   .FilterByStatus(args.Status)
                   .FilterByTimestamp(args.Timestamp)
                   .FilterIf(args.Type != null, x => x.Type == args.Type)
                   .FilterIf(!string.IsNullOrEmpty(args.Query), x =>
                       x.Title.ToLower().Contains(args.Query.ToLower()) ||
                       x.Description.ToLower().Contains(args.Query.ToLower()))
                   .Sort(args.OrderBy,
                       ("id", x => x.Id),
                       ("title", x => x.Title),
                       ("timestamp", x => x.Timestamp))
                   .PaginateAndSelect(args.Page, args.PageSize, x =>
                {
                    var ar = new AchievementResult(x);
                    if (!string.IsNullOrEmpty(x.Filename))
                        ar.ImageUrl = GenerateImageUrl(x.Id);
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

            var achievement = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable().FirstOrDefault(a => a.Id == id);

            // R# complaining that result always != null (that`s not true)
            #region No Resharper  
            if (achievement == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }
            #endregion

            if (!UserPermissions.IsAllowedToGet(User.Identity, achievement.Status, achievement.UserId))
                return Forbid();

            var result = new AchievementResult(achievement);
            if (!string.IsNullOrEmpty(achievement.Filename))
                result.ImageUrl = GenerateImageUrl(id);

            return Ok(result);
        }

        private string GenerateImageUrl(int id)
        {
            if (!string.IsNullOrWhiteSpace(_endpointConfig.ThumbnailUrlPattern))
            {
                // Generate thumbnail URL (if a thumbnail URL pattern is configured)
                return string.Format(_endpointConfig.ThumbnailUrlPattern, id);
            }
            else
            {
                // Return direct URL
                return $"{Request.Scheme}://{Request.Host}/api/images/{id}/";
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> PostAsync([FromBody] AchievementArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserPermissions.IsAllowedToCreate(User.Identity, args.Status))
                return Forbid();

            try
            {
                ArgumentHelper.GetAchievementTypeArgs(args);
            }
            catch (JsonSerializationException)
            {
                return BadRequest(new { Message = "The provided type arguments are invalid" });
            }

            var ev = new AchievementCreated
            {
                Id = _entityIndex.NextId(ResourceType.Achievement),
                UserId = User.Identity.GetUserIdentity(),
                Properties = args,
                Timestamp = DateTimeOffset.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return Created($"{Request.Scheme}://{Request.Host}/api/Achievements/{ev.Id}", ev.Id);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] AchievementArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_entityIndex.Exists(ResourceType.Achievement, id))
                return NotFound();

            if (!UserPermissions.IsAllowedToEdit(User.Identity, args.Status, _entityIndex.Owner(ResourceType.Achievement, id)))
                return Forbid();

            try
            {
                ArgumentHelper.GetAchievementTypeArgs(args);
            }
            catch (JsonSerializationException)
            {
                return BadRequest(new { Message = "The provided type arguments are invalid" });
            }
            
            var ev = new AchievementUpdated
            {
                Id = id,
                Properties = args,
                UserId = User.Identity.GetUserIdentity(),
                Timestamp = DateTime.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return NoContent();
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

            if (!_entityIndex.Exists(ResourceType.Achievement, id))
                return NotFound();

            var achievment = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable().FirstOrDefault(a => a.Id == id);
            if (!UserPermissions.IsAllowedToDelete(User.Identity, achievment.Status, achievment.UserId))
                return Forbid();

            var ev = new AchievementDeleted()
            {
                Id = id,
                UserId = User.Identity.GetUserIdentity(),
                Timestamp = DateTime.Now
            };

            await _eventStore.AppendEventAsync(ev);
            return NoContent();
        }

        [HttpGet("types")]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult Types() => Ok(Enum.GetNames(typeof(AchievementType)));
    }
}
