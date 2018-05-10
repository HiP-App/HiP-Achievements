using Microsoft.AspNetCore.Mvc;
using PaderbornUniversity.SILab.Hip.EventSourcing;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{

    public abstract class BaseController<TArgs> : Controller
    {
        /// <summary>
        /// ResourceType that should be used to create new entities
        /// </summary>
        protected abstract ResourceType ResourceType { get; }
    }

    public class ArgsValidationResult
    {
        public bool Success { get; set; }
        public IActionResult ActionResult { get; set; }
    }


}
