using AspireApp1.DbApi.Data;
using AspireApp1.DbApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

// Map controllers and a friendly root endpoint.
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.MapDefaultEndpoints();

// Apply any pending EF Core migrations at startup (useful for local dev).
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
    // If there are EF migrations in the assembly, apply them; otherwise create the schema directly.
    var migrations = db.Database.GetMigrations();
    if (migrations != null && migrations.Any())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }
}
catch (Exception ex)
{
    // swallow or log — during CI you may want to rethrow
    Console.Error.WriteLine($"Migration failed: {ex}");
}

app.Run();