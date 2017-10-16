using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AchievementsController : Controller
    {
        Achievement achievement1 = new Achievement
        {
            Id = 1,
            Description = "Visit 10 exhibits to get this achievement",
            Title = "Visit 10 exhibits",
            NextId = 2,
            Status = AchievementStatus.Published,
            Type = AchievementType.ExhibitVisited
        };
        Achievement achievement2 = new Achievement
        {
            Id = 2,
            Description = "Visit 20 exhibits to get this achievement",
            Title = "Visit 20 exhibits",
            NextId = 3,
            Status = AchievementStatus.Published,
            Type = AchievementType.ExhibitVisited
        };
        Achievement achievement3 = new Achievement
        {
            Id = 3,
            Description = "Visit 30 exhibits to get this achievement",
            Title = "Visit 40 exhibits",
            NextId = -1,
            Status = AchievementStatus.Published,
            Type = AchievementType.ExhibitVisited
        };
        Achievement achievement4 = new Achievement
        {
            Id = 1,
            Description = "Visit all exhibits on the Karls Route to get this achievement",
            Title = "Finish Karls Route",
            NextId = -1,
            Status = AchievementStatus.Unpublished,
            Type = AchievementType.RouteFinished
        };

        [HttpGet("ids")]
        [ProducesResponseType(200)]
        public IActionResult GetAllAchievements()
        {
            return Ok(new List<int> { 1, 2, 3, 4 });
        }

        [HttpGet]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAchievement(AchievementQueryArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievements = new[] { achievement1, achievement2, achievement3, achievement4 }.AsQueryable();

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
                   .PaginateAndSelect(args.Page, args.PageSize, x => new AchievementResult(x));

             if (result == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AchievementResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAchievementById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievements = new[] { achievement1, achievement2, achievement3, achievement4 };

            var result = achievements.FirstOrDefault(a => a.Id == id);
            if (result == null)
            {
                return NotFound(new { Message = "No Achievement could be found with this id" });
            }

            return Ok(new AchievementResult(result));
        }

    }
}
