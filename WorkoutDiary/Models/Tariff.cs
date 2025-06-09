using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutDiary.Models
{
    public class Tariff
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название тарифа")]
        public string Name { get; set; } = null!;

        [Display(Name = "Скорость (Мбит/с)")]
        public int Speed { get; set; }

        [Display(Name = "Цена (₽)")]
        public decimal Price { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        public List<Client> Clients { get; set; } = new();
    }
}
