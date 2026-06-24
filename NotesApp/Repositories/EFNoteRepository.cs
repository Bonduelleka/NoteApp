using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Repositories
{
    public class EFNoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public EFNoteRepository(AppDbContext context)
        {
            _context = context;
        }

        // Заметки
        public IEnumerable<Note> GetAllNotes()
        {
            return _context.Notes
                .Include(n => n.Categories)
                .OrderBy(n => n.Id)
                .ToList();
        }

        public Note? GetNoteById(int id)
        {
            return _context.Notes
                .Include(n => n.Categories)
                .FirstOrDefault(n => n.Id == id);
        }

        public bool AddNote(Note note)
        {
            _context.Notes.Add(note);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateNote(Note note)
        {
            var existing = _context.Notes.Find(note.Id);
            if (existing == null) return false;

            existing.Title = note.Title;
            existing.Description = note.Description;
            existing.Date = note.Date;

            return _context.SaveChanges() > 0;
        }

        public bool RemoveNote(Note note)
        {
            _context.Notes.Remove(note);
            return _context.SaveChanges() > 0;
        }

        // Категории
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories
                .Include(c => c.Notes)
                .OrderBy(c => c.Id)
                .ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _context.Categories
                .Include(c => c.Notes)
                .FirstOrDefault(c => c.Id == id);
        }

        public bool AddCategory(Category category)
        {
            _context.Categories.Add(category);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateCategory(Category category)
        {
            var existing = _context.Categories.Find(category.Id);
            if (existing == null) return false;

            existing.Title = category.Title;
            existing.Description = category.Description;

            return _context.SaveChanges() > 0;
        }

        public bool RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);
            return _context.SaveChanges() > 0;
        }

        // Фильтрация
        public IEnumerable<Note> GetNotesByDateRange(DateTime start, DateTime end)
        {
            return _context.Notes
                .Include(n => n.Categories)
                .Where(n => n.Date >= start && n.Date <= end)
                .OrderBy(n => n.Date)
                .ToList();
        }

        // Статистика
        public int GetNotesCount()
        {
            return _context.Notes.Count();
        }

        public (DateTime MinDate, DateTime MaxDate) GetDateRange()
        {
            var min = _context.Notes.Min(n => (DateTime?)n.Date) ?? DateTime.Now;
            var max = _context.Notes.Max(n => (DateTime?)n.Date) ?? DateTime.Now;
            return (min, max);
        }

        public List<string> GetAllUniqueTitles()
        {
            return _context.Notes
                .Select(n => n.Title)
                .Distinct()
                .ToList();
        }
    }
}