using DBooks.WebApi.Data;
using DBooks.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BooksController(LibraryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await db.Books.Include(b => b.OwnerUser).AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await db.Books.Include(b => b.OwnerUser).FirstOrDefaultAsync(b => b.Id == id) is { } b ? Ok(b) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Post(Book b)
    {
        db.Books.Add(b); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = b.Id }, b);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Book b)
    {
        if (id != b.Id) return BadRequest();
        db.Entry(b).State = EntityState.Modified;
        await db.SaveChangesAsync(); return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var b = await db.Books.FindAsync(id); if (b is null) return NotFound();
        db.Books.Remove(b); await db.SaveChangesAsync(); return NoContent();
    }
}

