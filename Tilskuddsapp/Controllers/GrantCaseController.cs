using Microsoft.AspNetCore.Mvc;

namespace Tilskuddsapp.Controllers
{
    public class GrantCaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
