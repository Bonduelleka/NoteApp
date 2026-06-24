using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;
using NotesApp.Repositories;

namespace NotesApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly INoteRepository _repo;

        public CategoryController(INoteRepository repo)
        {
            _repo = repo;
        }

        // GET: /Category/All
        public IActionResult All()
        {
            var categories = _repo.GetAllCategories();
            return View(categories);
        }

        // GET: /Category/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Category/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Category category)
        {
            // Проверка на дублирование по названию
            var existing = _repo.GetAllCategories()
                .Any(c => c.Title == category.Title);

            if (existing)
            {
                ModelState.AddModelError("Title", "Категория с таким названием уже существует");
            }

            if (ModelState.IsValid)
            {
                _repo.AddCategory(category);
                return RedirectToAction(nameof(All));
            }

            return View(category);
        }

        // GET: /Category/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _repo.GetCategoryById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _repo.UpdateCategory(category);
                return RedirectToAction(nameof(All));
            }

            return View(category);
        }

        // POST: /Category/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var category = _repo.GetCategoryById(id);
            if (category != null)
                _repo.RemoveCategory(category);

            return RedirectToAction(nameof(All));
        }

        // GET: /Category/Details/5
        public IActionResult Details(int id)
        {
            var category = _repo.GetCategoryById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}