using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Category> Categories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи многие-ко-многим
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Categories)
                .WithMany(c => c.Notes)
                .UsingEntity(j => j.ToTable("NoteCategories"));

            // Индекс для фильтрации по дате
            modelBuilder.Entity<Note>()
                .HasIndex(n => n.Date);
        }
    }
}