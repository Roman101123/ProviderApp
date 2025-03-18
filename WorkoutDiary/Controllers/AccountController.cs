using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkoutDiary.Data;
using WorkoutDiary.Models;

namespace WorkoutDiary.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;

            // Тестовый пользователь (можно убрать после проверки регистрации)
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User { Username = "test", PasswordHash = "test" });
                _context.SaveChanges();
            }
        }

        // Вход
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user != null && password == user.PasswordHash) // Упрощенная проверка
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("Index", "Workouts");
            }
            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        // Выход
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Регистрация (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Регистрация (POST)
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        {
            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View();
            }

            // Проверка, существует ли пользователь
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Пользователь с таким именем уже существует";
                return View();
            }

            // Создание нового пользователя
            var user = new User
            {
                Username = username,
                PasswordHash = password // В реальном проекте используйте хэширование
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Автоматическая авторизация после регистрации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Workouts");
        }
    }
}