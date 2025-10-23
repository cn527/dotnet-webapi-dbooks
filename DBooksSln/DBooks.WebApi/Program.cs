using DBooks.WebApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core con reintentos (maneja “SQL aún no está listo”)
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // opcional: servir Swagger también en prod para tu práctica
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Aplicar migraciones con reintentos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<LibraryDbContext>();

    const int maxAttempts = 10;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("Migraciones aplicadas correctamente.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "SQL no está listo (intento {Attempt}/{Max}). Esperando...", attempt, maxAttempts);
            if (attempt == maxAttempts) throw;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

app.Run();
