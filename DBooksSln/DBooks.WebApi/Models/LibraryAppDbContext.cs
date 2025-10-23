using DBooks.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DBooks.WebApi.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>()
            .HasMany(u => u.Books)
            .WithOne(bk => bk.OwnerUser)
            .HasForeignKey(bk => bk.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);   

        b.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);  

        b.Entity<Book>()
            .HasMany(bk => bk.Reviews)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);   
    }

}
