using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManagement.Models;
using BookManagement.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly BookContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(BookContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Book
        public async Task<IActionResult> Index(string q = "", int page = 1, int size = 10)
        {
            var queryBooks = _context.Books.Where(x => x.Name.Contains(q) || 
                                                       x.Field.Contains(q) || 
                                                       x.Author.Contains(q));
            var books = await PagingListDto<Book>.CreateAsync(queryBooks, page, size);
            return View(books);
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile? file, [Bind("Id,Name,Field,Author")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.Id = Guid.NewGuid();
                if (file != null) book.Thumbnail = CreateFile(file);
                book.CreatedDate = DateTime.Now;
                book.UpdatedDate = DateTime.Now;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile? file, [Bind("Id,Name,Field,Author,CreatedDate,Thumbnail")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null) book.Thumbnail = CreateFile(file);
                    book.UpdatedDate = DateTime.Now;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(Guid id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private string CreateFile(IFormFile file)
        {
            string oldFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string fileExt = Path.GetExtension(file.FileName);
            string fileName = oldFileName + DateTime.Now.ToString("-yyyyMMddhhmmss") + fileExt;

            var filePath = Path.Combine(_env.WebRootPath, "files", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return Path.Combine("files", fileName);
        }
    }
}
