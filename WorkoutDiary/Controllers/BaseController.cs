using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WorkoutDiary.Data;

namespace WorkoutDiary.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    ViewBag.UserAvatar = user.Avatar; // Передаем аватар в ViewBag
                }
            }
            base.OnActionExecuting(context);
        }
    }
}