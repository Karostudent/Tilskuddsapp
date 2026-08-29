using Microsoft.AspNetCore.Mvc;

namespace Tilskuddsapp.Controllers
{
    public class SupervisorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
