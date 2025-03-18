using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
	public class User
	{
		public int Id { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		public string PasswordHash { get; set; } // Храним хэш пароля
	}
}