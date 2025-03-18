using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
	public class Exercise
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
	}
}