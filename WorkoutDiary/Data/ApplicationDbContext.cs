using Microsoft.EntityFrameworkCore;
using WorkoutDiary.Models;

namespace WorkoutDiary.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }       // 👈 Добавлено
        public DbSet<Client> Clients { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальность имени тарифа
            modelBuilder.Entity<Tariff>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // Уникальность логина пользователя
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Пример начальных тарифов (если нужно)
            // modelBuilder.Entity<Tariff>().HasData(
            //     new Tariff { Id = 1, Name = "Базовый", Speed = 50, Price = 490 },
            //     new Tariff { Id = 2, Name = "Премиум", Speed = 100, Price = 890 }
            // );
        }
    }
}
