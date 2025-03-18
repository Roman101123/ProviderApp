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

            if (!_context.Exercises.Any())
            {
                _context.Exercises.AddRange(
                    new Exercise { Name = "Приседания" },
                    new Exercise { Name = "Жим лежа" },
                    new Exercise { Name = "Становая тяга" }
                );
                _context.SaveChanges();
            }
        }

        public IActionResult Index(DateTime? date)
        {
            var selectedDate = date ?? DateTime.Today;
            var workouts = _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .Where(w => w.Date.Date == selectedDate.Date && w.UserId == GetCurrentUserId())
                .ToList();
            ViewBag.SelectedDate = selectedDate;
            ViewBag.Exercises = _context.Exercises.ToList();
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
            _context.Workouts.Add(workout);
            _context.SaveChanges();
            return RedirectToAction("Index", new { date = Date });
        }

        [HttpPost]
        public IActionResult AddExercise(int workoutId, int exerciseId, int sets, int reps, double weight)
        {
            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workoutId,
                ExerciseId = exerciseId,
                Sets = sets,
                Reps = reps,
                Weight = weight
            };
            _context.WorkoutExercises.Add(workoutExercise);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
            ViewBag.Exercises = _context.Exercises.ToList(); // Для редактирования упражнений
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

        // Редактирование упражнения (GET)
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
            ViewBag.Exercises = _context.Exercises.ToList();
            return View(workoutExercise);
        }

        // Редактирование упражнения (POST)
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

        // Удаление упражнения
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

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}