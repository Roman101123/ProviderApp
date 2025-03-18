using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutDiary.Data;
using WorkoutDiary.Models;

namespace WorkoutDiary.Controllers
{
    [Authorize]
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Отображение списка упражнений
        public IActionResult Index()
        {
            var exercises = _context.Exercises.ToList();
            return View(exercises);
        }

        // Добавление нового упражнения (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Добавление нового упражнения (POST)
        [HttpPost]
        public IActionResult Create(string Name)
        {
            if (_context.Exercises.Any(e => e.Name == Name))
            {
                ViewBag.Error = "Упражнение с таким названием уже существует";
                return View();
            }
            var exercise = new Exercise { Name = Name };
            _context.Exercises.Add(exercise);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}