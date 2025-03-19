using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkoutDiary.Data;
using WorkoutDiary.Models;

namespace WorkoutDiary.Controllers
{
    [Authorize]
    public class JournalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JournalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Отображение списка записей с поиском
        public IActionResult Index(string search)
        {
            var entries = _context.JournalEntries
                .Include(e => e.Author)
                .Where(e => string.IsNullOrEmpty(search) || e.Title.Contains(search))
                .ToList();
            ViewBag.Search = search;
            return View(entries);
        }

        // Добавление новой записи (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Добавление новой записи (POST)
        [HttpPost]
        public async Task<IActionResult> Create(string title, string content, IFormFile image)
        {
            var userId = GetCurrentUserId();
            var entry = new JournalEntry
            {
                Title = title,
                Content = content,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    entry.Image = memoryStream.ToArray();
                }
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Просмотр отдельной записи
        [HttpGet]
        public IActionResult Details(int id)
        {
            var entry = _context.JournalEntries
                .Include(e => e.Author)
                .FirstOrDefault(e => e.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            ViewBag.IsAuthor = entry.AuthorId == GetCurrentUserId(); // Добавляем флаг в ViewBag
            return View(entry);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entry = _context.JournalEntries
                .Include(e => e.Author)
                .FirstOrDefault(e => e.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            if (entry.AuthorId != GetCurrentUserId())
            {
                return Forbid(); // Запрещаем доступ, если пользователь не автор
            }
            return View(entry);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string title, string content, IFormFile image)
        {
            var entry = _context.JournalEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            if (entry.AuthorId != GetCurrentUserId())
            {
                return Forbid();
            }

            entry.Title = title;
            entry.Content = content;

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    entry.Image = memoryStream.ToArray();
                }
            }

            _context.JournalEntries.Update(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = entry.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = _context.JournalEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
            {
                return NotFound();
            }
            if (entry.AuthorId != GetCurrentUserId())
            {
                return Forbid();
            }

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}