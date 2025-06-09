using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutDiary.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }  // 🔧 если хочешь выводить в Razor

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Тариф")]
        public int TariffId { get; set; }

        [ForeignKey("TariffId")]
        public Tariff Tariff { get; set; } = null!;

        [Display(Name = "Дата подключения")]
        public DateTime ConnectedAt { get; set; } // 🔁 Переименовано для совместимости с Razor
    }
}
