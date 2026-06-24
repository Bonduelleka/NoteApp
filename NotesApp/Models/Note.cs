using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите заголовок")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // Навигационное свойство для связи многие-ко-многим
        public List<Category> Categories { get; set; } = new();
    }
}