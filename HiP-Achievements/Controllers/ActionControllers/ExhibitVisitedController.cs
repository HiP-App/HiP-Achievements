using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;

using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers.ActionControllers
{
    public class ExhibitVisitedController : ActionBaseController<UserStore.ExhibitVisitedActionArgs>
    {
        protected readonly UserStoreService _userStoreService;

        public ExhibitVisitedController(UserStoreService userStoreService) : base(userStoreService)
        {
            _userStoreService = userStoreService;
        }

        protected override ResourceType ResourceType => ResourceTypes.ExhibitVisitedAction;

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

            var result = -1;
            try
            {
                result = await _userStoreService.ExhibitVisitedAction.PostManyAsync(args);
            }
            catch (SwaggerException ex)
            {
                return StatusCode(Int32.Parse(ex.StatusCode), ex.Response.Substring(1, ex.Response.Length - 2));
            }

            return StatusCode(201, result.ToString());
            
        }

        protected override async Task<ArgsValidationResult> ValidateActionArgs(ExhibitVisitedActionArgs args)
        {
            return null;
        }
    }
}
