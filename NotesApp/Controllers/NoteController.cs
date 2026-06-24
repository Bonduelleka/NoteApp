using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotesApp.Models;
using NotesApp.Repositories;

namespace NotesApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteRepository _repo;

        public NoteController(INoteRepository repo)
        {
            _repo = repo;
        }

        // GET: /Note/All?n=0&sort=Title
        public IActionResult All(int n = 0, string sort = null)
        {
            var notes = _repo.GetAllNotes().ToList();

            if (n > 0 && notes.Count > n)
                notes = notes.Take(n).ToList();

            if (!string.IsNullOrEmpty(sort))
            {
                notes = sort switch
                {
                    "Title" => notes.OrderBy(x => x.Title).ToList(),
                    "TitleDesc" => notes.OrderByDescending(x => x.Title).ToList(),
                    "Date" => notes.OrderBy(x => x.Date).ToList(),
                    "DateDesc" => notes.OrderByDescending(x => x.Date).ToList(),
                    _ => notes.OrderBy(x => x.Id).ToList()
                };
            }

            ViewData["Sort"] = sort;
            ViewData["N"] = n;

            return View(notes);
        }

        // GET: /Note/Add
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = new SelectList(_repo.GetAllCategories(), "Id", "Title");
            return View();
        }

        // POST: /Note/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Note note, int[] selectedCategories)
        {
            // Проверка на дублирование
            var existing = _repo.GetAllNotes()
                .Any(n => n.Title == note.Title &&
                         n.Description == note.Description &&
                         n.Date == note.Date);

            if (existing)
            {
                ModelState.AddModelError("", "Заметка с такими данными уже существует");
            }

            if (ModelState.IsValid)
            {
                // Добавляем выбранные категории
                if (selectedCategories != null)
                {
                    foreach (var catId in selectedCategories)
                    {
                        var category = _repo.GetCategoryById(catId);
                        if (category != null)
                            note.Categories.Add(category);
                    }
                }

                _repo.AddNote(note);
                return RedirectToAction(nameof(Stat));
            }

            ViewBag.Categories = new SelectList(_repo.GetAllCategories(), "Id", "Title", selectedCategories);
            return View(note);
        }

        // GET: /Note/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var note = _repo.GetNoteById(id);
            if (note == null)
                return NotFound();

            var selectedIds = note.Categories.Select(c => c.Id).ToArray();
            ViewBag.Categories = new SelectList(_repo.GetAllCategories(), "Id", "Title", selectedIds);

            return View(note);
        }

        // POST: /Note/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Note note, int[] selectedCategories)
        {
            if (id != note.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingNote = _repo.GetNoteById(id);
                if (existingNote == null)
                    return NotFound();

                existingNote.Title = note.Title;
                existingNote.Description = note.Description;
                existingNote.Date = note.Date;
                existingNote.Categories.Clear();

                if (selectedCategories != null)
                {
                    foreach (var catId in selectedCategories)
                    {
                        var category = _repo.GetCategoryById(catId);
                        if (category != null)
                            existingNote.Categories.Add(category);
                    }
                }

                _repo.UpdateNote(existingNote);
                return RedirectToAction(nameof(All));
            }

            ViewBag.Categories = new SelectList(_repo.GetAllCategories(), "Id", "Title", selectedCategories);
            return View(note);
        }

        // POST: /Note/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var note = _repo.GetNoteById(id);
            if (note != null)
                _repo.RemoveNote(note);

            return RedirectToAction(nameof(All));
        }

        // GET: /Note/Details/5
        public IActionResult Details(int id)
        {
            var note = _repo.GetNoteById(id);
            if (note == null)
                return NotFound();

            return View(note);
        }

        // GET: /Note/Stat
        public IActionResult Stat()
        {
            var notes = _repo.GetAllNotes().ToList();
            var dateRange = _repo.GetDateRange();

            ViewData["Count"] = _repo.GetNotesCount();
            ViewData["MinDate"] = dateRange.MinDate.ToString("yyyy-MM-dd");
            ViewData["MaxDate"] = dateRange.MaxDate.ToString("yyyy-MM-dd");
            ViewData["UniqueTitles"] = _repo.GetAllUniqueTitles();

            return View();
        }

        // GET: /Note/Export?n=0&sort=Title
        public IActionResult Export(int n = 0, string sort = null)
        {
            var notes = _repo.GetAllNotes().ToList();

            if (n > 0 && notes.Count > n)
                notes = notes.Take(n).ToList();

            if (!string.IsNullOrEmpty(sort))
            {
                notes = sort switch
                {
                    "Title" => notes.OrderBy(x => x.Title).ToList(),
                    "TitleDesc" => notes.OrderByDescending(x => x.Title).ToList(),
                    "Date" => notes.OrderBy(x => x.Date).ToList(),
                    "DateDesc" => notes.OrderByDescending(x => x.Date).ToList(),
                    _ => notes.OrderBy(x => x.Id).ToList()
                };
            }

            return Json(notes);
        }

        // GET: /Note/Filter
        public IActionResult Filter(DateTime? start, DateTime? end)
        {
            ViewBag.Start = start?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.End = end?.ToString("yyyy-MM-dd") ?? "";

            if (start.HasValue && end.HasValue)
            {
                var filtered = _repo.GetNotesByDateRange(start.Value, end.Value);
                return View(filtered.ToList());
            }

            return View(new List<Note>());
        }
    }
}