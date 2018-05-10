using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Threading.Tasks;
using ActionArgs = PaderbornUniversity.SILab.Hip.UserStore.ActionArgs;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    /// <summary>
    /// Base class for creating controllers for a specific action type
    /// </summary>
    /// <typeparam name="TArgs">Type of arguments</typeparam>

    [Authorize]
    [Route("api/Actions/[controller]")]
    public abstract class ActionBaseController<TArgs> : BaseController<TArgs> where TArgs : ActionArgs, new()
    {
        // ReSharper disable All
        protected readonly UserStoreService _userStoreService;
        // ReSharper Restore All


        public ActionBaseController(UserStoreService userStoreService)
        {
            _userStoreService = userStoreService;
        }

        [Obsolete("Use UserStore instead")]
        [HttpPost]
        [ProducesResponseType(typeof(int), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Post([FromBody] TArgs args)
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
            catch (SwaggerException ex)
            {
                return StatusCode(Int32.Parse(ex.StatusCode), ex.Response);
            }
        }
    }
}
