using DBooks.WebApi.Data;
using DBooks.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController(LibraryDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get() => Ok(await db.Users.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        await db.Users.FindAsync(id) is { } u ? Ok(u) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Post(User u)
    {
        db.Users.Add(u); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = u.Id }, u);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, User u)
    {
        if (id != u.Id) return BadRequest();
        db.Entry(u).State = EntityState.Modified;
        await db.SaveChangesAsync(); return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await db.Users.FindAsync(id); if (u is null) return NotFound();
        db.Users.Remove(u); await db.SaveChangesAsync(); return NoContent();
    }
}

