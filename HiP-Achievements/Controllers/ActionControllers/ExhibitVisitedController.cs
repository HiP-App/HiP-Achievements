using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;

using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.ActionControllers
{
    public class ExhibitVisitedController : ActionBaseController<ExhibitVisitedActionArgs>
    {
        private readonly UserStoreService _userStoreService;
        private readonly ExhibitsVisitedIndex _index;
        private readonly DataStoreService _dataStoreService;

        public ExhibitVisitedController(EventStoreService eventStore, InMemoryCache cache, DataStoreService dataStoreService, UserStoreService userStoreService) : base(eventStore, cache)
        {
            _userStoreService = userStoreService;
            _index = cache.Index<ExhibitsVisitedIndex>();
            _dataStoreService = dataStoreService;
        }

        protected override ResourceType ResourceType => ResourceTypes.ExhibitVisitedAction;

        [Obsolete("Use UserStore instead")]
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public override async Task<IActionResult> Post([FromBody] ExhibitVisitedActionArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (User.Identity.GetUserIdentity() == null)
                return Forbid();

            try
            {
                var id = await _userStoreService.ExhibitVisitedAction.PostAsync(new ExhibitVisitedActionArgs() { EntityId = args.EntityId });
                return Created($"{Request.Scheme}://{Request.Host}/api/Action/{id}", id);
            }
            catch (UserStore.SwaggerException ex)
            {
                return StatusCode(int.Parse(ex.StatusCode), ex.Response);
            }
        }

        /// <summary>
        /// Posts multiple ExhibitVisistedActions
        /// </summary>
        [Obsolete("Use UserStore instead")]
        [HttpPost("Many")]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostMany([FromBody] ExhibitVisitedActionsArgs args)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userStoreService.ExhibitVisitedAction.PostManyAsync(args);
                return StatusCode(201, result.ToString());
            }
            catch (UserStore.SwaggerException ex)
            {
                return StatusCode(int.Parse(ex.StatusCode), ex.Response.Substring(1, ex.Response.Length - 2));
            }
        }

        protected override async Task<ArgsValidationResult> ValidateActionArgs(ExhibitVisitedActionArgs args)
        {
            //check if the user has visited the exhibit already
            if (_index.Exists(User.Identity.GetUserIdentity(), args.EntityId))
            {
                return new ArgsValidationResult { ActionResult = BadRequest(new { Message = "The user has already visited this exhibit" }) };
            }

            //check if exhibits exists
            try
            {
                //this method throws a SwaggerException if the request fails 
                await _dataStoreService.Exhibits.GetByIdAsync(args.EntityId, null);
                return new ArgsValidationResult { Success = true };
            }
            catch (DataStore.SwaggerException)
            {
                return new ArgsValidationResult { ActionResult = NotFound(new { Message = "An exhibit with this id doesn't exist" }), Success = false };
            }
        }
    }
}
