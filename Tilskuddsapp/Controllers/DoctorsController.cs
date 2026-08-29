using Microsoft.AspNetCore.Mvc;

namespace Tilskuddsapp.Controllers
{
    public class DoctorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
