using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.Achievements.Model.Rest;
using PaderbornUniversity.SILab.Hip.UserStore;
using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ActionsController : Controller
    {
        private readonly UserStoreService _userStoreService;

        public ActionsController(UserStoreService userStoreService)
        {
            _userStoreService = userStoreService;
        }

        [Obsolete("Use UserStore instead")]
        [HttpGet]
        [ProducesResponseType(typeof(AllItemsResult<UserStore.ActionResult>), 200)]
        public async Task<IActionResult> GetAllActionsAsync()
        {
            var actions = await _userStoreService.Actions.GetAllActionsAsync();
            return Ok(actions);
        }
    }
}
