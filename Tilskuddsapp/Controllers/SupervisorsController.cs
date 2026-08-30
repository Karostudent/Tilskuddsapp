using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tilskuddsapp.Data;

namespace Tilskuddsapp.Controllers
{
    public class SupervisorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SupervisorIndex()
        {
            var supervisors = await _context.Supervisors
                .Include(s => s.GrantCases)
                .Where(s => s.Active)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(supervisors);
        }
    }
}
