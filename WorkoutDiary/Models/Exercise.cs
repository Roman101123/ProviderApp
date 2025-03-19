using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? UserId { get; set; } // Nullable, для упражнений по умолчанию UserId будет null
        public User User { get; set; }

        public bool IsDefault { get; set; } // Указывает, является ли упражнение общедоступным по умолчанию
    }
}