using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using WorkoutDiary.Data;
using WorkoutDiary.Models;

namespace WorkoutDiary.Controllers
{
    [Authorize]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? date, int? month, int? year)
        {
            var selectedDate = date ?? DateTime.Today;
            var currentMonth = month ?? selectedDate.Month;
            var currentYear = year ?? selectedDate.Year;

            var workouts = _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .Where(w => w.Date.Date == selectedDate.Date && w.UserId == GetCurrentUserId())
                .ToList();

            var workoutDates = _context.Workouts
                .Where(w => w.UserId == GetCurrentUserId() && w.Date.Year == currentYear && w.Date.Month == currentMonth)
                .Select(w => w.Date.Date)
                .Distinct()
                .ToList();

            ViewBag.SelectedDate = selectedDate;
            ViewBag.Exercises = _context.Exercises
                .Where(e => e.IsDefault || e.UserId == GetCurrentUserId())
                .ToList();
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.CurrentYear = currentYear;
            ViewBag.WorkoutDates = workoutDates;

            return View(workouts);
        }

        [HttpPost]
        public IActionResult Create(string Note, DateTime Date)
        {
            var workout = new Workout
            {
                Note = Note,
                Date = Date,
                UserId = GetCurrentUserId()
            };
            try
            {
                _context.Workouts.Add(workout);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return BadRequest("Ошибка при сохранении тренировки: " + ex.InnerException?.Message);
            }
            return RedirectToAction("Index", new { date = Date });
        }

        [HttpPost]
        public IActionResult AddExercise(int workoutId, string exerciseName, int sets, int reps, double weight, DateTime Date)
        {
            // Валидация названия
            if (string.IsNullOrWhiteSpace(exerciseName) || exerciseName.Length < 3)
            {
                return BadRequest("Название упражнения должно содержать минимум 3 символа");
            }

            // Поиск упражнения
            var userId = GetCurrentUserId();
            var exercise = _context.Exercises
                .FirstOrDefault(e =>
                    e.Name.ToLower() == exerciseName.Trim().ToLower() &&
                    (e.UserId == userId || e.IsDefault)
                );

            // Создание нового упражнения
            if (exercise == null)
            {
                exercise = new Exercise
                {
                    Name = exerciseName.Trim(),
                    UserId = userId,
                    IsDefault = false
                };
                _context.Exercises.Add(exercise);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    return BadRequest("Ошибка при сохранении упражнения: " + ex.InnerException?.Message);
                }
            }

            // Добавление в тренировку
            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workoutId,
                ExerciseId = exercise.Id,
                Sets = sets,
                Reps = reps,
                Weight = weight
            };

            _context.WorkoutExercises.Add(workoutExercise);
            _context.SaveChanges();

            return RedirectToAction("Index", new { date = Date });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var workout = _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefault(w => w.Id == id && w.UserId == GetCurrentUserId());
            if (workout == null)
            {
                return NotFound();
            }
            ViewBag.Exercises = _context.Exercises
                .Where(e => e.IsDefault || e.UserId == GetCurrentUserId())
                .ToList();
            return View(workout);
        }

        [HttpPost]
        public IActionResult Edit(Workout workout)
        {
            var existingWorkout = _context.Workouts
                .FirstOrDefault(w => w.Id == workout.Id && w.UserId == GetCurrentUserId());
            if (existingWorkout == null)
            {
                return NotFound();
            }
            existingWorkout.Note = workout.Note;
            _context.SaveChanges();
            return RedirectToAction("Index", new { date = existingWorkout.Date });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var workout = _context.Workouts
                .FirstOrDefault(w => w.Id == id && w.UserId == GetCurrentUserId());
            if (workout == null)
            {
                return NotFound();
            }
            _context.Workouts.Remove(workout);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditExercise(int id)
        {
            var workoutExercise = _context.WorkoutExercises
                .Include(we => we.Exercise)
                .FirstOrDefault(we => we.Id == id);
            if (workoutExercise == null || _context.Workouts.FirstOrDefault(w => w.Id == workoutExercise.WorkoutId && w.UserId == GetCurrentUserId()) == null)
            {
                return NotFound();
            }
            ViewBag.Exercises = _context.Exercises
                .Where(e => e.IsDefault || e.UserId == GetCurrentUserId())
                .ToList();
            return View(workoutExercise);
        }

        [HttpPost]
        public IActionResult EditExercise(WorkoutExercise workoutExercise)
        {
            var existingExercise = _context.WorkoutExercises
                .FirstOrDefault(we => we.Id == workoutExercise.Id);
            if (existingExercise == null || _context.Workouts.FirstOrDefault(w => w.Id == workoutExercise.WorkoutId && w.UserId == GetCurrentUserId()) == null)
            {
                return NotFound();
            }
            existingExercise.Sets = workoutExercise.Sets;
            existingExercise.Reps = workoutExercise.Reps;
            existingExercise.Weight = workoutExercise.Weight;
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = workoutExercise.WorkoutId });
        }

        [HttpGet]
        public IActionResult DeleteExercise(int id)
        {
            var workoutExercise = _context.WorkoutExercises
                .FirstOrDefault(we => we.Id == id);
            if (workoutExercise == null || _context.Workouts.FirstOrDefault(w => w.Id == workoutExercise.WorkoutId && w.UserId == GetCurrentUserId()) == null)
            {
                return NotFound();
            }
            _context.WorkoutExercises.Remove(workoutExercise);
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = workoutExercise.WorkoutId });
        }

        [HttpPost]
        public IActionResult UpdateNote(int id, string note)
        {
            var existingWorkout = _context.Workouts
                .FirstOrDefault(w => w.Id == id && w.UserId == GetCurrentUserId());

            if (existingWorkout == null) return NotFound();

            existingWorkout.Note = note;
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [AcceptVerbs("Post")]
        public IActionResult UpdateExercise([FromBody] WorkoutExerciseUpdateDto dto)
        {
            var exercise = _context.WorkoutExercises
                .Include(we => we.Workout)
                .FirstOrDefault(we => we.Id == dto.Id);

            if (exercise == null || exercise.Workout.UserId != GetCurrentUserId())
                return NotFound();

            exercise.Sets = dto.Sets;
            exercise.Reps = dto.Reps;
            exercise.Weight = dto.Weight;

            try
            {
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
    public class WorkoutExerciseUpdateDto
    {
        public int Id { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}