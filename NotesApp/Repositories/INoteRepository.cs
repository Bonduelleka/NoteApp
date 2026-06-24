using NotesApp.Models;

namespace NotesApp.Repositories
{
    public interface INoteRepository
    {
        // Методы для заметок
        IEnumerable<Note> GetAllNotes();
        Note? GetNoteById(int id);
        bool AddNote(Note note);
        bool UpdateNote(Note note);
        bool RemoveNote(Note note);

        // Методы для категорий
        IEnumerable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        bool AddCategory(Category category);
        bool UpdateCategory(Category category);
        bool RemoveCategory(Category category);

        // Фильтрация по датам
        IEnumerable<Note> GetNotesByDateRange(DateTime start, DateTime end);

        // Статистика
        int GetNotesCount();
        (DateTime MinDate, DateTime MaxDate) GetDateRange();
        List<string> GetAllUniqueTitles();
    }
}