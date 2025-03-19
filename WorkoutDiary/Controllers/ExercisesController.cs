using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        // Отображение списка упражнений (по умолчанию + пользовательские)
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var exercises = _context.Exercises
                .Where(e => e.IsDefault || e.UserId == userId)
                .ToList();
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
            var userId = GetCurrentUserId();
            if (_context.Exercises.Any(e => e.Name == Name && (e.IsDefault || e.UserId == userId)))
            {
                ViewBag.Error = "Упражнение с таким названием уже существует";
                return View();
            }
            var exercise = new Exercise
            {
                Name = Name,
                UserId = userId, // Привязываем к текущему пользователю
                IsDefault = false // Пользовательское упражнение
            };
            _context.Exercises.Add(exercise);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}