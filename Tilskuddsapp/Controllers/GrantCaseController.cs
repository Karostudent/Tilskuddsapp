using Microsoft.AspNetCore.Mvc;

namespace Tilskuddsapp.Controllers
{
    public class GrantCasesController : Controller
    {
        public IActionResult GrantCaseIndex()
        {
            return View();
        }
    }
}
