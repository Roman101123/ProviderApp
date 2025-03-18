using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WorkoutDiary.Data;

namespace WorkoutDiary
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// ��������� ������� MVC
			builder.Services.AddControllersWithViews();

			// ��������� ��������� ���� ������ SQLite
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite("Data Source=workoutdiary.db"));

			// ��������� ����������� ����� cookies
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Account/Login";
					options.LogoutPath = "/Account/Logout";
				});

			var app = builder.Build();

			// ������������ middleware
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication(); // ��������� ����� UseAuthorization
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}