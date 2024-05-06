using BookManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookManagement.Hubs;
using BookManagement.Dtos;
using System.Drawing;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<BookHub> _hubContext;

        public PageController(BookContext context, IWebHostEnvironment env, IHubContext<BookHub> hubContext)
        {
            _context = context;
            _env = env;
            _hubContext = hubContext;
        }

        // GET: api/Page
        [HttpGet]
        public IActionResult Get(Guid bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
                var pages = _context.Pages.Where(x => x.BookId == bookId)
                                          .OrderBy(x => x.PageNumber)
                                          .ToList();
                book.Pages = pages;
            }

            return Ok(book);
        }

        // GET api/Page/5
        [HttpGet("{id}")]
        public IActionResult GetOne(Guid id)
        {
            var page = _context.Pages.Find(id);
            if (page == null) return NotFound();
            return Ok(page);
        }

        // POST api/Page
        [HttpPost]
        public IActionResult Create(IFormFile? file, [FromForm] Page page)
        {
            var book = _context.Books.Find(page.BookId);
            if (book == null) return NotFound();

            page.Id = Guid.NewGuid();
            if (file != null) page.FilePath = CreateFile(file);
            _context.Add(page);

            book.UpdatedDate = DateTime.Now;
            _context.SaveChanges();
            _hubContext.Clients.All.SendAsync("BookUpdated", book.Id);
            return Ok(page);
        }

        // PUT api/Page/5
        [HttpPut("{id}")]
        public IActionResult Edit(Guid id, IFormFile? file, [FromForm] Page pageInfo)
        {
            var book = _context.Books.Find(pageInfo.BookId);
            if (book == null) return NotFound();

            var page = _context.Pages.Find(id);
            if (page == null) return NotFound();

            page.PageNumber = pageInfo.PageNumber;
            page.BookId = pageInfo.BookId;
            if (file != null) page.FilePath = CreateFile(file);

            book.UpdatedDate = DateTime.Now;
            _context.SaveChanges();
            _hubContext.Clients.All.SendAsync("BookUpdated", book.Id);
            return Ok(page);
        }

        // DELETE api/Page/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var page = _context.Pages.Find(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
                var book = _context.Books.Find(page.BookId);
                if (book != null) book.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
                _hubContext.Clients.All.SendAsync("BookUpdated", page.BookId);
            }
            return Ok();
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

        [HttpGet("bookmarks")]
        [Authorize]
        public IActionResult GetUserBookmarks()
        {
            var user = _context.Users.Include(x => x.Pages).FirstOrDefault(x => x.Id.Equals(Guid.Parse(User.Identity.Name)));
            return Ok(user.Pages.ToList());
        }

        [HttpPost("bookmark/{id}")]
        [Authorize]
        public IActionResult Bookmark(Guid id)
        {
            var page = _context.Pages.Find(id);
            if (page == null) return NotFound();

            var user = _context.Users.Include(x => x.Pages).FirstOrDefault(x => x.Id.Equals(Guid.Parse(User.Identity.Name)));
            var bookmarkedPage = user.Pages.FirstOrDefault(x => x.BookId.Equals(page.BookId));
            if (bookmarkedPage != null)
            {
                user.Pages.Remove(bookmarkedPage);
                if (!page.Id.Equals(bookmarkedPage.Id)) user.Pages.Add(page);
            }
            else user.Pages.Add(page);
            _context.SaveChanges();
            return Ok(page);
        }

        [HttpPost("test")]
        public async Task<IActionResult> test(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    string rs = reader.ReadToEnd();
                    return Ok(rs);
                }
            }
        }
    }
}
