using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Username { get; set; } = null!;

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Пароль (хэш)")]
        public string PasswordHash { get; set; } = null!;

        [Display(Name = "Аватар")]
        public byte[]? Avatar { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Тариф")]
        public int? CurrentTariffId { get; set; }

        public Tariff? CurrentTariff { get; set; }
    }
}
