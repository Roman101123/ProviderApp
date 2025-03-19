using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public byte[]? Image { get; set; } // Изображение как бинарные данные, nullable

        [Required]
        public int AuthorId { get; set; }

        public User Author { get; set; } // Связь с пользователем

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}