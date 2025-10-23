using DBooks.WebApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;

    public int OwnerUserId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public User? OwnerUser { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}



