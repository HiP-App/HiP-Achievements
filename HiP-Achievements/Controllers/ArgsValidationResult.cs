using Microsoft.AspNetCore.Mvc;

namespace PaderbornUniversity.SILab.Hip.Achievements.Controllers
{
    public class ArgsValidationResult
    {
        public bool Success { get; set; }
        public IActionResult ActionResult { get; set; }
    }
}
