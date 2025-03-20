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

        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; } // Добавлено

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Exercise>()
               .HasIndex(e => new { e.Name, e.UserId, e.IsDefault })
               .IsUnique();

            // Начальные данные: 10 упражнений по умолчанию
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Приседания", IsDefault = true },
                new Exercise { Id = 2, Name = "Жим лежа", IsDefault = true },
                new Exercise { Id = 3, Name = "Становая тяга", IsDefault = true },
                new Exercise { Id = 4, Name = "Подтягивания", IsDefault = true },
                new Exercise { Id = 5, Name = "Отжимания", IsDefault = true },
                new Exercise { Id = 6, Name = "Жим стоя", IsDefault = true },
                new Exercise { Id = 7, Name = "Тяга штанги в наклоне", IsDefault = true },
                new Exercise { Id = 8, Name = "Скручивания", IsDefault = true },
                new Exercise { Id = 9, Name = "Планка", IsDefault = true },
                new Exercise { Id = 10, Name = "Бег", IsDefault = true }
            );
        }
    }
}