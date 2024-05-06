using BookManagement.Dtos;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookContext _context;

        public HomeController(BookContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string q = "", int page = 1, int size = 10)
        {
            ViewData["Title"] = "Home Page";

            var queryBooks = _context.Books
                .Where(x => x.Name.Contains(q) ||
                            x.Field.Contains(q) ||
                            x.Author.Contains(q))
                .OrderByDescending(x => x.UpdatedDate);

            var books = await PagingListDto<Book>.CreateAsync(queryBooks, page, size);
            return View(books);
        }

        [Authorize]
        public async Task<IActionResult> Bookmark(string q = "", int page = 1, int size = 10)
        {
            ViewData["Title"] = "Bookmark";

            var queryBooks = from b in _context.Books
                             join p in _context.Pages on b.Id equals p.BookId
                             where p.Users.Any(u => u.Id == Guid.Parse(User.Identity.Name))
                             orderby b.UpdatedDate descending
                             select b;

            var books = await PagingListDto<Book>.CreateAsync(queryBooks, page, size);
            return View("Index", books);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
