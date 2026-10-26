using CollegeApi.Data;
using CollegeApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CollegeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeDB")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/students", async (CollegeDbContext db) =>
    Results.Ok(await db.Students.AsNoTracking().OrderBy(student => student.Id).ToListAsync()));

app.MapGet("/api/student/{id:int}", async (int id, CollegeDbContext db) =>
{
    var student = await db.Students.AsNoTracking()
        .FirstOrDefaultAsync(item => item.Id == id);
    return student is null ? Results.NotFound() : Results.Ok(student);
});

app.MapPost("/api/student", async (StudentCreate model, CollegeDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(model.FirstName) ||
        string.IsNullOrWhiteSpace(model.LastName))
    {
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });
    }

    if (model.GroupId.HasValue &&
        !await db.Groups.AnyAsync(group => group.Id == model.GroupId.Value))
    {
        return Results.BadRequest(new { message = "Указанная группа не найдена." });
    }

    var student = new Student
    {
        FirstName = model.FirstName.Trim(),
        LastName = model.LastName.Trim(),
        GroupId = model.GroupId
    };

    db.Students.Add(student);
    await db.SaveChangesAsync();

    return Results.Created($"/api/student/{student.Id}", student);
});

app.MapPut("/api/student/{id:int}", async (int id, StudentCreate model, CollegeDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound();

    if (string.IsNullOrWhiteSpace(model.FirstName) ||
        string.IsNullOrWhiteSpace(model.LastName))
    {
        return Results.BadRequest(new { message = "Имя и фамилия обязательны." });
    }

    if (model.GroupId.HasValue &&
        !await db.Groups.AnyAsync(group => group.Id == model.GroupId.Value))
    {
        return Results.BadRequest(new { message = "Указанная группа не найдена." });
    }

    student.FirstName = model.FirstName.Trim();
    student.LastName = model.LastName.Trim();
    student.GroupId = model.GroupId;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/student/{id:int}", async (int id, CollegeDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null)
        return Results.NotFound();

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/groups", async (CollegeDbContext db) =>
    Results.Ok(await db.Groups.AsNoTracking().OrderBy(group => group.Id).ToListAsync()));

app.MapFallbackToFile("index.html");
app.Run();
