﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PaderbornUniversity.SILab.Hip.Achievements.Core;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Entity;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Events;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly EventStoreClient _eventStore;
        private readonly CacheDatabaseManager _db;
        private readonly UploadFilesConfig _filesConfig;

        public ImageController(EventStoreClient eventStore, CacheDatabaseManager db, IOptions<UploadFilesConfig> filesConfig)
        {
            _eventStore = eventStore;
            _db = db;
            _filesConfig = filesConfig.Value;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PutImageById(int id, IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var achievement = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable()
                .FirstOrDefault(a => a.Id == id);

            if (achievement == null)
                return NotFound(new { Message = "No Achievement could be found with this id" });

            if (!UserPermissions.IsAllowedToCreateImage(User.Identity, achievement.UserId))
            {
                return Forbid();
            }

            var extension = file.FileName.Split('.').Last();
            if (!_filesConfig.SupportedFormats.Contains(extension))
                return BadRequest(new { Message = $"Extension '{extension}' is not supported" });

            var filePath = Path.Combine(_filesConfig.Path, id + "." + extension);

            //Delete old file
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
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

        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public IActionResult GetImageById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var achievement = _db.Database.GetCollection<Achievement>(ResourceType.Achievement.Name).AsQueryable()
                .FirstOrDefault(a => a.Id == id);

            if (achievement == null)
                return NotFound(new { Message = "No Achievement could be found with this id" });

            if (!System.IO.File.Exists(achievement.Filename))
                return NotFound(new { Message = "The image could not be found" });

            //figure out the mime type
            new FileExtensionContentTypeProvider().TryGetContentType(achievement.Filename, out string mimeType);
            mimeType = mimeType ?? "application/octet-stream";
            return File(new FileStream(achievement.Filename, FileMode.Open), mimeType, Path.GetFileName(achievement.Filename));
        }
    }
}
