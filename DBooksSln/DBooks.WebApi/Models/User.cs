namespace DBooks.WebApi.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
