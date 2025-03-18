using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
	public class Workout
	{
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; }

		public string Note { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }

		public List<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
	}
}