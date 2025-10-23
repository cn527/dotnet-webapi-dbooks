using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBooks.WebApi.Data;     
namespace DBooks.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ReviewsController : ControllerBase
    {
        private readonly LibraryDbContext _db;

        public ReviewsController(LibraryDbContext db) => _db = db;

        [HttpGet("~/api/books/{bookId:int}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetForBook(int bookId)
        {
            var exists = await _db.Books.AsNoTracking().AnyAsync(b => b.Id == bookId);
            if (!exists) return NotFound($"Book {bookId} not found.");

            var list = await _db.Reviews
                .AsNoTracking()
                .Where(r => r.BookId == bookId) // <-- usa FK
                .ToListAsync();

            return Ok(list);
        }


        [HttpPost("~/api/books/{bookId:int}/reviews")]
        public async Task<ActionResult<Review>> CreateForBook(int bookId, [FromBody] Review body)
        {
            if (!await _db.Books.AsNoTracking().AnyAsync(b => b.Id == bookId))
                return NotFound($"Book {bookId} not found.");
            if (!await _db.Users.AsNoTracking().AnyAsync(u => u.Id == body.UserId))
                return NotFound($"User {body.UserId} not found.");

            var entity = new Review
            {
                Rating = body.Rating,
                Comment = body.Comment,
                UserId = body.UserId,
                BookId = bookId
            };

            _db.Reviews.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Review>> GetById(int id)
        {
            var r = await _db.Reviews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return r is null ? NotFound() : Ok(r);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Review body)
        {
            var r = await _db.Reviews.FindAsync(id);
            if (r is null) return NotFound();

            if (!await _db.Books.AsNoTracking().AnyAsync(b => b.Id == body.BookId))
                return NotFound($"Book {body.BookId} not found.");
            if (!await _db.Users.AsNoTracking().AnyAsync(u => u.Id == body.UserId))
                return NotFound($"User {body.UserId} not found.");

            r.Rating = body.Rating;
            r.Comment = body.Comment;
            r.UserId = body.UserId;
            r.BookId = body.BookId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("~/api/books/{bookId:int}/reviews/{id:int}")]
        public async Task<IActionResult> DeleteFromBook(int bookId, int id)
        {
            var review = await _db.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.BookId == bookId);

            if (review is null)
                return NotFound();

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

