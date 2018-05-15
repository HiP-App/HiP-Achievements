using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileStream = System.IO.FileStream;
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly EventStoreService _eventStore;
        private readonly IMongoDbContext _db;
        private readonly UploadFilesConfig _filesConfig;
        private readonly EntityIndex _entityIndex;
        private readonly ThumbnailService.ThumbnailService _thumbnailService;

        public ImageController(EventStoreService eventStore, IMongoDbContext db,
            IOptions<UploadFilesConfig> filesConfig, InMemoryCache cache,
            ThumbnailService.ThumbnailService thumbnailService)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _db = db;
            _thumbnailService = thumbnailService;
            _filesConfig = filesConfig.Value;
            _entityIndex = cache.Index<EntityIndex>();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutImageById(int id, IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_entityIndex.Exists(ResourceTypes.Achievement, id))
                return NotFound(new { Message = "No Achievement could be found with this id" });


            if (!UserPermissions.IsAllowedToCreateImage(User.Identity, _entityIndex.Owner(ResourceTypes.Achievement, id)))
            {
                return Forbid();
            }

            var extension = file.FileName.Split('.').Last();
            if (!_filesConfig.SupportedFormats.Contains(extension))
                return BadRequest(new { Message = $"Extension '{extension}' is not supported" });

            var filePath = Path.Combine(_filesConfig.Path, id + "." + extension);

            //Delete old file and inform ThumbnailService
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                await _thumbnailService.TryClearThumbnailCacheAsync(id);
            }

            Directory.CreateDirectory(_filesConfig.Path);

            //Create new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var ev = new AchievementImageUpdated
            {
                Id = id,
                File = filePath,
                Timestamp = DateTimeOffset.Now,
                UserId = User.Identity.GetUserIdentity()
            };

            await _eventStore.AppendEventAsync(ev);

            return NoContent();

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetImageById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var achievement = _db.GetCollection<Achievement>(ResourceTypes.Achievement).AsQueryable()
                .FirstOrDefault(a => a.Id == id);

            if (achievement == null)
                return NotFound(new { Message = "No Achievement could be found with this id" });

            if (!System.IO.File.Exists(achievement.Filename))
                return NotFound(new { Message = "The image could not be found" });

            //Figure out the mime type
            new FileExtensionContentTypeProvider().TryGetContentType(achievement.Filename, out string mimeType);
            mimeType = mimeType ?? "application/octet-stream";
            return File(new FileStream(achievement.Filename, FileMode.Open), mimeType, Path.GetFileName(achievement.Filename));
        }
    }
}
