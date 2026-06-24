using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название категории")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Навигационное свойство
        public List<Note> Notes { get; set; } = new();
    }
}