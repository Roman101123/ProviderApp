using Microsoft.AspNetCore.Mvc;
using WorkoutDiary.Data;
using WorkoutDiary.Models;
using System.Linq;

namespace WorkoutDiary.Controllers
{
    public class TariffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TariffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tariffs = _context.Tariffs.ToList();
            return View(tariffs); // => Views/Tariffs/Index.cshtml
        }
    }
}
