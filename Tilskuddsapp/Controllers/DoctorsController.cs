using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tilskuddsapp.Data;

namespace Tilskuddsapp.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DoctorIndex()
        {
            var doctors = await _context.Doctors
                .Include(d => d.GrantCases)
                .Where(d => d.Active)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(doctors);
        }
    }
}
