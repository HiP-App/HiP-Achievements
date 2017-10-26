using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{

    public abstract class BaseController<TArgs> : Controller
    {
        /// <summary>
        /// Determines whether the provided arguments are valid according to the specific requirements 
        /// </summary>
        /// <param name="args">Arguments to be validated</param>
        /// <returns>Validation result</returns>
        protected abstract Task<ArgsValidationResult> ValidateActionArgs(TArgs args);
    }

    public class ArgsValidationResult
    {
        public bool Success { get; set; }
        public IActionResult ActionResult { get; set; }
    }

    
}
