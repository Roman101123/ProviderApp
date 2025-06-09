using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutDiary.Data;
using WorkoutDiary.Models;

namespace WorkoutDiary.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var clients = _context.Clients
                .Include(c => c.Tariff)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                clients = clients.Where(c => c.FullName.Contains(search));
            }

            ViewBag.Search = search;
            return View(clients.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Tariffs = _context.Tariffs.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                client.ConnectedAt = DateTime.Now;
                _context.Clients.Add(client);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tariffs = _context.Tariffs.ToList();
            return View(client);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
                return NotFound();

            ViewBag.Tariffs = _context.Tariffs.ToList();
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Clients.Update(client);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Tariffs = _context.Tariffs.ToList();
            return View(client);
        }

        public IActionResult Details(int id)
        {
            var client = _context.Clients
                .Include(c => c.Tariff)
                .FirstOrDefault(c => c.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var client = _context.Clients.Find(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
